using Microsoft.EntityFrameworkCore;

using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context) { }

        public async Task<Booking?> GetWithDetailsAsync(int bookingId, CancellationToken ct = default) =>
            await _dbSet
                .Include(b => b.Customer)
                .Include(b => b.Vehicle)
                .Include(b => b.Inspections)
                .Include(b => b.Payments).ThenInclude(p => p.Refunds)
                .Include(b => b.Modifications)
                .Include(b => b.Invoice).ThenInclude(i => i!.Items)
                .AsSplitQuery()
                .FirstOrDefaultAsync(b => b.Id == bookingId, ct);

        /// A vehicle is free when no blocking booking overlaps the range.
        /// Blocking = Confirmed, PickedUp, or Pending that has not yet expired.
        public async Task<bool> IsVehicleAvailableAsync(
            int vehicleId, DateTime start, DateTime end,
            int? excludeBookingId = null, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            var hasConflict = await _dbSet.AnyAsync(b =>
                b.VehicleId == vehicleId
                && b.StartAt < end
                && b.EndAt > start
                && (excludeBookingId == null || b.Id != excludeBookingId)
                && (b.Status == BookingStatus.Confirmed
                    || b.Status == BookingStatus.PickedUp
                    || (b.Status == BookingStatus.Pending && b.PendingExpiresAt > now)), ct);

            return !hasConflict;
        }

        public async Task<IReadOnlyList<Booking>> GetByCustomerAsync(int customerId, CancellationToken ct = default) =>
            await _dbSet.AsNoTracking()
                .Include(b => b.Vehicle)
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.StartAt)
                .ToListAsync(ct);

        public async Task<IReadOnlyList<Booking>> GetExpiredPendingAsync(DateTime now, CancellationToken ct = default) =>
            await _dbSet
                .Where(b => b.Status == BookingStatus.Pending && b.PendingExpiresAt != null && b.PendingExpiresAt <= now)
                .ToListAsync(ct);

        public async Task<IReadOnlyList<Booking>> GetByStatusAsync(BookingStatus status, CancellationToken ct = default) =>
            await _dbSet.AsNoTracking()
                .Where(b => b.Status == status)
                .OrderBy(b => b.StartAt)
                .ToListAsync(ct);
    }
}

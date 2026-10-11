using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository;

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

    /// Free when no blocking booking overlaps [start, end) and no maintenance falls in the range.
    /// Blocking bookings: Confirmed, PickedUp, or Pending that has not yet expired.
    public async Task<bool> IsVehicleAvailableAsync(
        int vehicleId, DateTime start, DateTime end,
        int? excludeBookingId = null, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        var bookingConflict = await _dbSet.AnyAsync(b =>
            b.VehicleId == vehicleId
            && b.StartAt < end
            && b.EndAt > start
            && (excludeBookingId == null || b.Id != excludeBookingId)
            && (b.Status == BookingStatus.Confirmed
                || b.Status == BookingStatus.PickedUp
                || (b.Status == BookingStatus.Pending && b.PendingExpiresAt > now)), ct);

        if (bookingConflict) return false;

        var fromDate = DateOnly.FromDateTime(start);
        var toDate = DateOnly.FromDateTime(end);

        // InProgress maintenance blocks everything from its start date. Scheduled blocks its own day.
        var maintenanceConflict = await _context.MaintenanceRecords.AnyAsync(m =>
            m.VehicleId == vehicleId
            && ((m.Status == MaintenanceStatus.InProgress && m.ScheduledDate <= toDate)
                || (m.Status == MaintenanceStatus.Scheduled
                    && m.ScheduledDate >= fromDate && m.ScheduledDate <= toDate)), ct);

        return !maintenanceConflict;
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
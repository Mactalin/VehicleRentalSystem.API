using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository;

public class BookingDriverRepository : GenericRepository<BookingDriver>, IBookingDriverRepository
{
    public BookingDriverRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<BookingDriver>> GetByBookingIdAsync(int bookingId, CancellationToken ct = default)
        => await _context.BookingDrivers
            .Include(bd => bd.Driver)
            .Where(bd => bd.BookingId == bookingId)
            .ToListAsync(ct);

    public async Task<IEnumerable<BookingDriver>> GetByDriverIdAsync(int driverId, CancellationToken ct = default)
        => await _context.BookingDrivers
            .Include(bd => bd.Booking)
            .Where(bd => bd.DriverId == driverId)
            .ToListAsync(ct);

    public async Task<BookingDriver?> GetActiveAssignmentAsync(int bookingId, int driverId, CancellationToken ct = default)
        => await _context.BookingDrivers
            .FirstOrDefaultAsync(bd => bd.BookingId == bookingId
                                    && bd.DriverId == driverId
                                    && bd.IsActive, ct);
}
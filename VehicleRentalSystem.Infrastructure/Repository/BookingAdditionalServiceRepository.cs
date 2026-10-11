using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository;

public class BookingAdditionalServiceRepository : GenericRepository<BookingAdditionalService>, IBookingAdditionalServiceRepository
{
    public BookingAdditionalServiceRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<BookingAdditionalService>> GetByBookingIdAsync(int bookingId, CancellationToken ct = default)
        => await _db.BookingAdditionalServices
            .Include(b => b.AdditionalService)
            .Where(b => b.BookingId == bookingId)
            .ToListAsync(ct);
}
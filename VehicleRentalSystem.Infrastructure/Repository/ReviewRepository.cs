using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository;

public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
    public ReviewRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<Review>> GetByVehicleIdAsync(int vehicleId, CancellationToken ct = default)
        => await _context.Reviews
            .Where(r => r.VehicleId == vehicleId)
            .OrderByDescending(r => r.PostedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Review>> GetByDriverIdAsync(int driverId, CancellationToken ct = default)
        => await _context.Reviews
            .Where(r => r.DriverId == driverId)
            .OrderByDescending(r => r.PostedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Review>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default)
        => await _context.Reviews
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.PostedAt)
            .ToListAsync(ct);

    public async Task<double> GetAverageRatingForVehicleAsync(int vehicleId, CancellationToken ct = default)
    {
        var ratings = await _context.Reviews
            .Where(r => r.VehicleId == vehicleId)
            .Select(r => r.Rating)
            .ToListAsync(ct);

        return ratings.Count == 0 ? 0 : Math.Round(ratings.Average(), 2);
    }

    public async Task<double> GetAverageRatingForDriverAsync(int driverId, CancellationToken ct = default)
    {
        var ratings = await _context.Reviews
            .Where(r => r.DriverId == driverId)
            .Select(r => r.Rating)
            .ToListAsync(ct);

        return ratings.Count == 0 ? 0 : Math.Round(ratings.Average(), 2);
    }

    public async Task<bool> HasReviewedBookingAsync(int bookingId, CancellationToken ct = default)
        => await _context.Reviews.AnyAsync(r => r.BookingId == bookingId, ct);
}
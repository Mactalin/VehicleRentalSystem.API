using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Interface_Repository
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetByVehicleIdAsync(int vehicleId, CancellationToken ct = default);
        Task<IEnumerable<Review>> GetByDriverIdAsync(int driverId, CancellationToken ct = default);
        Task<IEnumerable<Review>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default);
        Task<double> GetAverageRatingForVehicleAsync(int vehicleId, CancellationToken ct = default);
        Task<double> GetAverageRatingForDriverAsync(int driverId, CancellationToken ct = default);
        Task<bool> HasReviewedBookingAsync(int bookingId, CancellationToken ct = default);
    }
}
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Interface_Repository
{
    public interface IBookingDriverRepository : IGenericRepository<BookingDriver>
    {
        Task<IEnumerable<BookingDriver>> GetByBookingIdAsync(int bookingId, CancellationToken ct = default);
        Task<IEnumerable<BookingDriver>> GetByDriverIdAsync(int driverId, CancellationToken ct = default);
        Task<BookingDriver?> GetActiveAssignmentAsync(int bookingId, int driverId, CancellationToken ct = default);
    }
}
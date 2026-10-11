using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Interface_Repository
{
    public interface IBookingAdditionalServiceRepository : IGenericRepository<BookingAdditionalService>
    {
        Task<IEnumerable<BookingAdditionalService>> GetByBookingIdAsync(int bookingId, CancellationToken ct = default);
    }
}
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;


namespace VehicleRentalSystem.Domain.Interface_Repository;

public interface IBookingRepository : IGenericRepository<Booking>
{
    Task<Booking?> GetWithDetailsAsync(int bookingId, CancellationToken ct = default);

    /// True when no Confirmed/PickedUp (or unexpired Pending) booking overlaps [start, end):
    /// existing.StartAt < end AND existing.EndAt > start
    Task<bool> IsVehicleAvailableAsync(int vehicleId, DateTime start, DateTime end,
        int? excludeBookingId = null, CancellationToken ct = default);

    Task<IReadOnlyList<Booking>> GetByCustomerAsync(int customerId, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetExpiredPendingAsync(DateTime now, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetByStatusAsync(BookingStatus status, CancellationToken ct = default);
}
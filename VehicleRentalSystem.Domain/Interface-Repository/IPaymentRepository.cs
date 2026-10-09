using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Interface_Repository;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<IReadOnlyList<Payment>> GetByBookingAsync(int bookingId, CancellationToken ct = default);
    Task<IReadOnlyList<Payment>> GetByCustomerAsync(int customerId, CancellationToken ct = default);
    Task<Payment?> GetByGatewayReferenceAsync(string reference, CancellationToken ct = default);
}
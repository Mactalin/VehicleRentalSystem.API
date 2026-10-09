using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Interface_Repository;

public interface IInvoiceRepository : IGenericRepository<Invoice>
{
    Task<Invoice?> GetByBookingAsync(int bookingId, CancellationToken ct = default);
    Task<string> GenerateInvoiceNumberAsync(CancellationToken ct = default);
}
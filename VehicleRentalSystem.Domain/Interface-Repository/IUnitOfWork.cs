namespace VehicleRentalSystem.Domain.Interface_Repository;

public interface IUnitOfWork : IDisposable
{
    ICustomerRepository Customers { get; }
    IKycRepository KycDocuments { get; }
    IBookingRepository Bookings { get; }
    IInspectionRepository Inspections { get; }
    IPaymentRepository Payments { get; }
    IInvoiceRepository Invoices { get; }
    IVehicleRepository Vehicles { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}
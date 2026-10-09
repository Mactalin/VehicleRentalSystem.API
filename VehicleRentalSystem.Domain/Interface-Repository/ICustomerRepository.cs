using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;


namespace VehicleRentalSystem.Domain.Interface_Repository;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<Customer?> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<Customer?> GetWithDocumentsAsync(int customerId, CancellationToken ct = default);
    Task<bool> LicenceNumberExistsAsync(string licenceNumber, CancellationToken ct = default);
    Task<(IReadOnlyList<Customer> Items, int Total)> GetPagedAsync(
        int page, int pageSize, VerificationStatus? verification, CustomerStatus? status,
        string? search, CancellationToken ct = default);
}
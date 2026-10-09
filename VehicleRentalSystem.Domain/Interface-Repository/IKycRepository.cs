using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Interface_Repository;

public interface IKycRepository : IGenericRepository<KycDocument>
{
    Task<IReadOnlyList<KycDocument>> GetByCustomerAsync(int customerId, CancellationToken ct = default);
}
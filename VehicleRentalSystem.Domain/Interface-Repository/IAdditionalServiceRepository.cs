using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Interface_Repository
{
    public interface IAdditionalServiceRepository : IGenericRepository<AdditionalService>
    {
        Task<IEnumerable<AdditionalService>> GetActiveAsync(CancellationToken ct = default);
        Task<IEnumerable<AdditionalService>> GetByTypeAsync(ServiceType type, CancellationToken ct = default);
    }
}
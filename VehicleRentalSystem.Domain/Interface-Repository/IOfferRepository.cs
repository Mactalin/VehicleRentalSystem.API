using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Interface_Repository
{
    public interface IOfferRepository : IGenericRepository<Offer>
    {
        Task<IEnumerable<Offer>> GetRunningAsync(CancellationToken ct = default);
    }
}
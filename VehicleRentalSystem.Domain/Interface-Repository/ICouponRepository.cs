using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Interface_Repository
{
    public interface ICouponRepository : IGenericRepository<Coupon>
    {
        Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct = default);
        Task<IEnumerable<Coupon>> GetActiveAsync(CancellationToken ct = default);
        Task<int> GetUsageCountAsync(int couponId, int customerId, CancellationToken ct = default);
    }
}
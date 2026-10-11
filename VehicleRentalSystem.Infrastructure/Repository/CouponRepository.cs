using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository;

public class CouponRepository : GenericRepository<Coupon>, ICouponRepository
{
    public CouponRepository(AppDbContext db) : base(db) { }

    public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct = default)
        => await _db.Coupons
            .Include(c => c.Usages)
            .FirstOrDefaultAsync(c => c.Code == code, ct);

    public async Task<IEnumerable<Coupon>> GetActiveAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return await _db.Coupons
            .Where(c => c.IsActive
                     && c.Status == CouponStatus.Active
                     && c.ValidFrom <= now
                     && c.ValidTo >= now)
            .OrderBy(c => c.ValidTo)
            .ToListAsync(ct);
    }

    public async Task<int> GetUsageCountAsync(int couponId, int customerId, CancellationToken ct = default)
        => await _db.CouponUsages
            .CountAsync(u => u.CouponId == couponId && u.CustomerId == customerId, ct);
}
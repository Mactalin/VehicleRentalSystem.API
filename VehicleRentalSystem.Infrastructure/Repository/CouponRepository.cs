using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.RepositoryInterfaces;
using VehicleRentalSystem.Infrastructure.Data;
using static Azure.Core.HttpHeader;

namespace VehicleRentalSystem.Infrastructure.Repositories;

public class CouponRepository : GenericRepository<Coupon>, ICouponRepository
{
    public CouponRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Coupon?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Code == code);
    }

    public async Task<IEnumerable<Coupon>> GetActiveCouponsAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(c => c.IsActive && c.ValidFrom <= now && c.ValidTo >= now)
            .ToListAsync();
    }
}

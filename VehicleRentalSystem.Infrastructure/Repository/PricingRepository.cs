using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository;

public class PricingRepository : IPricingRepository
{
    private readonly AppDbContext _context;

    public PricingRepository(AppDbContext context) => _context = context;

    public async Task<CategoryPricing?> GetPricingAsync(int categoryId, CancellationToken ct = default) =>
        await _context.CategoryPricings
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)       // newest row wins if a category ever has several
            .FirstOrDefaultAsync(p => p.CategoryId == categoryId, ct);

    public async Task<decimal> GetSeasonalMultiplierAsync(
        int categoryId, DateOnly pickupDate, CancellationToken ct = default)
    {
        var multipliers = await _context.SeasonalPricings
            .AsNoTracking()
            .Where(s => s.CategoryId == categoryId
                        && s.IsActive
                        && s.StartDate <= pickupDate
                        && s.EndDate >= pickupDate)
            .Select(s => s.Multiplier)
            .ToListAsync(ct);

        return multipliers.Count == 0 ? 1m : multipliers.Max();
    }
}
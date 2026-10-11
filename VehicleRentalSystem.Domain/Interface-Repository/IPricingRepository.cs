using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Interface_Repository;

public interface IPricingRepository
{
    Task<CategoryPricing?> GetPricingAsync(int categoryId, CancellationToken ct = default);

    /// 1.0 when no seasonal rule covers the date. If several do, the highest multiplier wins.
    Task<decimal> GetSeasonalMultiplierAsync(int categoryId, DateOnly pickupDate, CancellationToken ct = default);
}
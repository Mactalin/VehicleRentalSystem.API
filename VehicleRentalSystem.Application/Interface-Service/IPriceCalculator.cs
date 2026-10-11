using VehicleRentalSystem.Application.Dtos.Bookings;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Application.Interface_Service;

public interface IPriceCalculator
{
    PriceQuoteDto Calculate(
        CategoryPricing pricing,
        DateTime startAt,
        DateTime endAt,
        decimal addOnAmount = 0m,
        decimal discountAmount = 0m,
        decimal seasonalMultiplier = 1m);
}
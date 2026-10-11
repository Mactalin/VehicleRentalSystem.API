using VehicleRentalSystem.Application.Common;
using VehicleRentalSystem.Application.Dtos.Bookings;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Exceptions;

namespace VehicleRentalSystem.Application.Services;

public class PriceCalculator : IPriceCalculator
{
    private readonly BillingSettings _settings;

    public PriceCalculator(BillingSettings settings) => _settings = settings;

    /// Under 24h: hourly. 24h or more: per started day, and from 7 days the weekly price
    /// is used for full weeks (never costing more than plain daily pricing).
    public PriceQuoteDto Calculate(
        CategoryPricing pricing, DateTime startAt, DateTime endAt,
        decimal addOnAmount = 0m, decimal discountAmount = 0m, decimal seasonalMultiplier = 1m)
    {
        if (endAt <= startAt)
            throw new DomainException("End time must be after the start time.");

        var duration = endAt - startAt;
        var totalHours = (int)Math.Ceiling(duration.TotalHours);

        if (totalHours < _settings.MinimumRentalHours)
            throw new DomainException($"The minimum rental period is {_settings.MinimumRentalHours} hour(s).");

        if (duration.TotalDays > _settings.MaximumRentalDays)
            throw new DomainException($"The maximum rental period is {_settings.MaximumRentalDays} days.");

        var m = seasonalMultiplier <= 0 ? 1m : seasonalMultiplier;

        RateType rateType;
        decimal rate;
        int units;
        decimal baseAmount;

        if (totalHours < 24 && pricing.HourlyPrice > 0)
        {
            rateType = RateType.Hourly;
            rate = Math.Round(pricing.HourlyPrice * m, 2);
            units = totalHours;
            baseAmount = units * rate;
        }
        else
        {
            var days = (int)Math.Ceiling(totalHours / 24.0);
            rateType = RateType.Daily;
            rate = Math.Round(pricing.DailyPrice * m, 2);
            units = days;
            baseAmount = days * rate;

            if (pricing.WeeklyPrice > 0 && days >= 7)
            {
                var weekly = Math.Round(pricing.WeeklyPrice * m, 2);
                var withWeekly = (days / 7) * weekly + (days % 7) * rate;
                baseAmount = Math.Min(baseAmount, withWeekly);
            }
        }

        baseAmount = Math.Round(baseAmount, 2);

        var subtotal = Math.Max(0m, baseAmount + addOnAmount - discountAmount);
        var tax = Math.Round(subtotal * _settings.GstRate, 2);
        var total = subtotal + tax;

        return new PriceQuoteDto
        {
            RateType = rateType,
            RateApplied = rate,
            Units = units,
            BaseAmount = baseAmount,
            AddOnAmount = addOnAmount,
            DiscountAmount = discountAmount,
            TaxAmount = tax,
            TotalAmount = total,
            SecurityDeposit = pricing.SecurityDeposit,
            AdvanceAmount = Math.Round(total * _settings.AdvancePercent, 2)
        };
    }
}

using VehicleRentalSystem.Application.Common;
using VehicleRentalSystem.Application.Dtos.Bookings;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Exceptions;

namespace VehicleRentalSystem.Application.Services
{
    public class PriceCalculator : IPriceCalculator
    {
        private readonly BillingSettings _settings;

        public PriceCalculator(BillingSettings settings) => _settings = settings;

        /// Under 24 hours: charged hourly. 24 hours or more: charged per started day.
        /// subtotal = base + add-ons - discount;  GST on subtotal;  deposit is separate and untaxed.
        public PriceQuoteDto Calculate(
            Vehicle vehicle, DateTime startAt, DateTime endAt,
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

            RateType rateType;
            decimal rate;
            int units;

            if (totalHours < 24 && vehicle.HourlyRate > 0)
            {
                rateType = RateType.Hourly;
                rate = vehicle.HourlyRate;
                units = totalHours;
            }
            else
            {
                rateType = RateType.Daily;
                rate = vehicle.DailyRate;
                units = (int)Math.Ceiling(totalHours / 24.0);
            }

            var effectiveRate = Math.Round(rate * seasonalMultiplier, 2);
            var baseAmount = Math.Round(units * effectiveRate, 2);

            var subtotal = Math.Max(0m, baseAmount + addOnAmount - discountAmount);
            var tax = Math.Round(subtotal * _settings.GstRate, 2);
            var total = subtotal + tax;

            return new PriceQuoteDto
            {
                RateType = rateType,
                RateApplied = effectiveRate,
                Units = units,
                BaseAmount = baseAmount,
                AddOnAmount = addOnAmount,
                DiscountAmount = discountAmount,
                TaxAmount = tax,
                TotalAmount = total,
                SecurityDeposit = vehicle.SecurityDeposit,
                AdvanceAmount = Math.Round(total * _settings.AdvancePercent, 2)
            };
        }
    }
}

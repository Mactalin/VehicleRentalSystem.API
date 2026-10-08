using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Category;
using VehicleRentalSystem.Application.Exceptions;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services
{
    public class PricingService : IPricingService
    {
        private const int HoursPerDay = 24;
        private const int HoursPerWeek = 168;
        private const int MaxRentalDays = 365;

        private readonly IVehicleRepository _vehicles;
        private readonly ICategoryRepository _categories;

        public PricingService(IVehicleRepository vehicles, ICategoryRepository categories)
        {
            _vehicles = vehicles;
            _categories = categories;
        }

        public async Task<PriceQuoteDto> GetQuoteAsync(int vehicleId, DateTime from, DateTime to)
        {
            if (to <= from)
                throw new BadRequestException("Return time must be after the pickup time.");
            if ((to - from).TotalDays > MaxRentalDays)
                throw new BadRequestException($"Rental period cannot exceed {MaxRentalDays} days.");

            var vehicle = await _vehicles.GetByIdAsync(vehicleId)
                          ?? throw new NotFoundException("Vehicle not found.");

            var category = await _categories.GetByIdWithPricingAsync(vehicle.CategoryId)
                           ?? throw new NotFoundException("Vehicle category not found.");

            var price = category.Pricing
                        ?? throw new BadRequestException($"Pricing is not configured for category '{category.Name}'.");

            // Split the duration into weeks / days / hours (any part-hour is charged as a full hour)
            var totalHours = (int)Math.Ceiling((to - from).TotalHours);
            var weeks = totalHours / HoursPerWeek;
            var remaining = totalHours % HoursPerWeek;
            var days = remaining / HoursPerDay;
            var hours = remaining % HoursPerDay;

            // Cheapest sensible combination: leftover hours never cost more than a day,
            // and leftover days never cost more than a week
            var hoursCharge = Math.Min(hours * price.HourlyPrice, price.DailyPrice);
            var weekRemainder = Math.Min(days * price.DailyPrice + hoursCharge, price.WeeklyPrice);
            var baseAmount = weeks * price.WeeklyPrice + weekRemainder;

            // Seasonal multiplier: average across each calendar day in the rental
            var seasons = category.SeasonalPricings.Where(s => s.IsActive).ToList();
            var startDate = DateOnly.FromDateTime(from);
            var endDate = DateOnly.FromDateTime(to);

            decimal multiplierSum = 0;
            var dayCount = 0;
            for (var d = startDate; d <= endDate; d = d.AddDays(1))
            {
                multiplierSum += seasons
                    .Where(s => s.StartDate <= d && s.EndDate >= d)
                    .Select(s => s.Multiplier)
                    .DefaultIfEmpty(1m)
                    .Max();
                dayCount++;
            }

            var averageMultiplier = Math.Round(multiplierSum / dayCount, 4);
            var adjustment = Math.Round(baseAmount * (averageMultiplier - 1m), 2);
            var rentalAmount = baseAmount + adjustment;

            return new PriceQuoteDto
            {
                VehicleId = vehicle.Id,
                RegistrationNumber = vehicle.RegistrationNumber,
                CategoryName = category.Name,
                From = from,
                To = to,
                TotalHours = totalHours,
                Weeks = weeks,
                Days = days,
                Hours = hours,
                BaseAmount = baseAmount,
                SeasonalMultiplier = averageMultiplier,
                SeasonalAdjustment = adjustment,
                RentalAmount = rentalAmount,
                SecurityDeposit = price.SecurityDeposit,
                TotalPayable = rentalAmount + price.SecurityDeposit
            };
        }
    }
}

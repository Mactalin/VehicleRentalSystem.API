using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Common;

namespace VehicleRentalSystem.Domain.Entities
{
    public class VehicleCategory : BaseEntity
    {
        public string Name { get; set; } = string.Empty;   // Car, SUV, Van, Bike
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public CategoryPricing? Pricing { get; set; }
        public ICollection<SeasonalPricing> SeasonalPricings { get; set; } = new List<SeasonalPricing>();
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Common;

namespace VehicleRentalSystem.Domain.Entities
{
    public class SeasonalPricing : BaseEntity
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;   // e.g. "Pongal Season", "Summer Peak"
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal Multiplier { get; set; } = 1.0m;    // 1.25 = 25% higher than base price
        public bool IsActive { get; set; } = true;

        public VehicleCategory Category { get; set; } = null!;
    }
}

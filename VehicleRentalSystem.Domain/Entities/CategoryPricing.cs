using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Common;

namespace VehicleRentalSystem.Domain.Entities
{
    public class CategoryPricing : BaseEntity
    {
        public int CategoryId { get; set; }
        public decimal HourlyPrice { get; set; }
        public decimal DailyPrice { get; set; }
        public decimal WeeklyPrice { get; set; }
        public decimal SecurityDeposit { get; set; }

        public VehicleCategory Category { get; set; } = null!;
    }
}

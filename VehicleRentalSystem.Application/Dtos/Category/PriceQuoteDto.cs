using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Category
{
    public class PriceQuoteDto
    {
        public int VehicleId { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public int TotalHours { get; set; }
        public int Weeks { get; set; }
        public int Days { get; set; }
        public int Hours { get; set; }

        public decimal BaseAmount { get; set; }
        public decimal SeasonalMultiplier { get; set; }
        public decimal SeasonalAdjustment { get; set; }
        public decimal RentalAmount { get; set; }      // BaseAmount + SeasonalAdjustment
        public decimal SecurityDeposit { get; set; }
        public decimal TotalPayable { get; set; }      // RentalAmount + SecurityDeposit (before tax/discounts)
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Bookings;

namespace VehicleRentalSystem.Application.Dtos.Search
{

    public class VehicleSearchResultDto
    {
        public int VehicleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int BranchId { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal DailyRate { get; set; }
        public decimal SecurityDeposit { get; set; }
        public PriceQuoteDto? Quote { get; set; }     // filled only when From and To are given
    }
}

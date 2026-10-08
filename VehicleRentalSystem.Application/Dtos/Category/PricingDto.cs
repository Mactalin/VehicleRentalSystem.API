using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Category
{
    public class PricingDto
    {
        [Range(0, 1000000)] public decimal HourlyPrice { get; set; }
        [Range(0, 1000000)] public decimal DailyPrice { get; set; }
        [Range(0, 1000000)] public decimal WeeklyPrice { get; set; }
        [Range(0, 1000000)] public decimal SecurityDeposit { get; set; }
    }
}

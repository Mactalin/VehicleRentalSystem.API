using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Category
{
    public class SaveSeasonalPricingDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        [Range(0.1, 10)]
        public decimal Multiplier { get; set; } = 1.0m;

        public bool IsActive { get; set; } = true;
    }
}

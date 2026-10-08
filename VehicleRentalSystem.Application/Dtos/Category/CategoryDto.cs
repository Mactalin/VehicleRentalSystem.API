using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public PricingDto? Pricing { get; set; }
    }
}

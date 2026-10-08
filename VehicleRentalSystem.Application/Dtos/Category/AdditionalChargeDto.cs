using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Category
{
    public class AdditionalChargeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public AdditionalChargeType ChargeType { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}

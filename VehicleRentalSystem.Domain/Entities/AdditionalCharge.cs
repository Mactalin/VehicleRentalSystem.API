using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Common;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Entities
{
    public class AdditionalCharge : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public AdditionalChargeType ChargeType { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

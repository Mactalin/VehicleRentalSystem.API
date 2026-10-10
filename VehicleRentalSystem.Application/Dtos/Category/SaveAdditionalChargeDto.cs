using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Category
{
    public class SaveAdditionalChargeDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public AdditionalChargeType ChargeType { get; set; }

        [Range(0, 1000000)]
        public decimal Amount { get; set; }

        [StringLength(300)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

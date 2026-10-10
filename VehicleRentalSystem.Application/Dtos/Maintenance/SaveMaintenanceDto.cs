using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Maintenance
{
    public class SaveMaintenanceDto
    {
        [Range(1, int.MaxValue)]
        public int VehicleId { get; set; }

        public MaintenanceType Type { get; set; }

        [Required, StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public DateOnly ScheduledDate { get; set; }

        [Range(0, 10000000)]
        public decimal Cost { get; set; }

        [StringLength(150)]
        public string? VendorName { get; set; }
    }
}

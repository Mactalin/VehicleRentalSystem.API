using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Common;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Entities
{
    public class MaintenanceRecord : BaseEntity
    {
        public int VehicleId { get; set; }
        public MaintenanceType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateOnly ScheduledDate { get; set; }
        public DateOnly? CompletedDate { get; set; }
        public decimal Cost { get; set; }
        public string? VendorName { get; set; }
        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Scheduled;

        public Vehicle Vehicle { get; set; } = null!;
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Maintenance
{
    public class MaintenanceDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string? RegistrationNumber { get; set; }
        public MaintenanceType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateOnly ScheduledDate { get; set; }
        public DateOnly? CompletedDate { get; set; }
        public decimal Cost { get; set; }
        public string? VendorName { get; set; }
        public MaintenanceStatus Status { get; set; }
    }
}

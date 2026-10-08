using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Maintenance
{
    public class UpdateMaintenanceStatusDto
    {
        public MaintenanceStatus Status { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Vehicle
{
    public class UpdateVehicleStatusDto
    {
        public VehicleStatus Status { get; set; }
    }
}

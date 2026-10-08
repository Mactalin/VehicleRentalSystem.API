using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Maintenance
{
    public class CompleteMaintenanceDto
    {
        public DateOnly? CompletedDate { get; set; }

        [Range(0, 10000000)]
        public decimal Cost { get; set; }
    }
}

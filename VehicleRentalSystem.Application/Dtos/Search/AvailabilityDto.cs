using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Search
{
    public class AvailabilityDto
    {
        public int VehicleId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool IsAvailable { get; set; }
    }
}

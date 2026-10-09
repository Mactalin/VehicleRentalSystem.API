using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Bookings
{

    public class InspectionDto
    {
        public int Id { get; set; }
        public InspectionType Type { get; set; }
        public int OdometerReading { get; set; }
        public decimal FuelLevelPercent { get; set; }
        public string? DamageNotes { get; set; }
        public string? PhotoUrls { get; set; }
        public decimal DamageCharge { get; set; }
        public DateTime InspectedAt { get; set; }
    }
}

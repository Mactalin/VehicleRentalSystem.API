using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Bookings
{
    public class PickupDto
    {
        [Range(0, int.MaxValue)] public int OdometerReading { get; set; }
        [Range(0, 100)] public decimal FuelLevelPercent { get; set; }
        [StringLength(1000)] public string? DamageNotes { get; set; }
        [StringLength(2000)] public string? PhotoUrls { get; set; }
    }
}

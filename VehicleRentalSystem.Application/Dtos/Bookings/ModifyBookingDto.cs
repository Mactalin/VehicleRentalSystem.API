using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Bookings
{
    public class ModifyBookingDto
    {
        public DateTime NewStartAt { get; set; }
        public DateTime NewEndAt { get; set; }
        [StringLength(500)] public string? Reason { get; set; }
    }
}

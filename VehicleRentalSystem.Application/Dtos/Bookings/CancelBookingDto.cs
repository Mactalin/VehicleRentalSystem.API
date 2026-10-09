using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Bookings
{

    public class CancelBookingDto
    {
        [Required, StringLength(500)] public string Reason { get; set; } = string.Empty;
    }
}

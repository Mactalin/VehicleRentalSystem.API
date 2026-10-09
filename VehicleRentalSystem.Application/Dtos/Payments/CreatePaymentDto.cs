using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Payments
{
    // The amount is NOT accepted from the client. The server works it out from the booking.
    public class CreatePaymentDto
    {
        [Range(1, int.MaxValue)] public int BookingId { get; set; }
        public PaymentType Type { get; set; }
        public PaymentMethod Method { get; set; }
    }
}

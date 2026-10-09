using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Bookings
{
    public class CancellationResultDto
    {
        public int BookingId { get; set; }
        public BookingStatus Status { get; set; }
        public decimal RefundAmount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Payments;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Bookings
{
    public class ReturnSummaryDto
    {
        public int BookingId { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime ReturnedAt { get; set; }
        public decimal LateFee { get; set; }
        public decimal DamageCharge { get; set; }
        public SettlementDto Settlement { get; set; } = new();
    }
}

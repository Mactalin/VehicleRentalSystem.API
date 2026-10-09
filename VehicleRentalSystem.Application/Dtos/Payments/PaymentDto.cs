using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Payments
{

    public class PaymentDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public PaymentType Type { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string? GatewayReference { get; set; }
        public string? FailureReason { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal RefundedAmount { get; set; }
        public List<RefundDto> Refunds { get; set; } = new();
    }
}

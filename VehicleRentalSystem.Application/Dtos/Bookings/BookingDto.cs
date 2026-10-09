using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Application.Dtos.Payments;

namespace VehicleRentalSystem.Application.Dtos.Bookings
{
    public class BookingDto
    {
        public int Id { get; set; }
        public string BookingNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string VehicleRegistration { get; set; } = string.Empty;
        public int PickupBranchId { get; set; }
        public int DropBranchId { get; set; }

        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public DateTime? ActualPickupAt { get; set; }
        public DateTime? ActualReturnAt { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime? PendingExpiresAt { get; set; }

        public RateType RateType { get; set; }
        public decimal RateApplied { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal AddOnAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal SecurityDeposit { get; set; }
        public decimal AdvanceAmount { get; set; }
        public string? CouponCode { get; set; }

        public decimal LateFee { get; set; }
        public decimal DamageCharge { get; set; }

        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }

        public List<InspectionDto> Inspections { get; set; } = new();
        public List<PaymentDto> Payments { get; set; } = new();
    }
}

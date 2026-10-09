

using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Entities;

public class Payment : BaseEntity
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public PaymentType Type { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string? GatewayReference { get; set; }      // transaction / order id from gateway
    public string? FailureReason { get; set; }
    public DateTime? PaidAt { get; set; }

    public ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    public decimal RefundedAmount => Refunds.Where(r => r.Status == RefundStatus.Processed).Sum(r => r.Amount);
    public decimal RefundableAmount => Status == PaymentStatus.Success ? Amount - RefundedAmount : 0m;
}
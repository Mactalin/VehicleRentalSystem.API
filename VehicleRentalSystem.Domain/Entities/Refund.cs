

using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Entities;

public class Refund : BaseEntity
{
    public int PaymentId { get; set; }
    public Payment Payment { get; set; } = null!;

    public int BookingId { get; set; }

    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;   // cancellation, deposit return, etc.
    public RefundStatus Status { get; set; } = RefundStatus.Pending;
    public string? GatewayReference { get; set; }
    public DateTime? ProcessedAt { get; set; }
}
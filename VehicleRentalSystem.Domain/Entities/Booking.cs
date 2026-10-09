
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Exceptions;

namespace VehicleRentalSystem.Domain.Entities;

public class Booking : BaseEntity
{
    public string BookingNumber { get; set; } = string.Empty;

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int VehicleId { get; set; }            // Anbu's Vehicle
    public Vehicle Vehicle { get; set; } = null!;

    public int PickupBranchId { get; set; }       // Anbu's Branch
    public int DropBranchId { get; set; }

    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public DateTime? ActualPickupAt { get; set; }
    public DateTime? ActualReturnAt { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime? PendingExpiresAt { get; set; }   // unpaid bookings auto-expire

    // ---- Price snapshot (frozen at booking time, later rate changes don't affect it) ----
    public RateType RateType { get; set; }
    public decimal RateApplied { get; set; }          // per hour or per day
    public decimal BaseAmount { get; set; }
    public decimal AddOnAmount { get; set; }          // Lenin: drivers / GPS / accessories
    public decimal DiscountAmount { get; set; }       // Lenin: coupons
    public decimal TaxAmount { get; set; }            // GST
    public decimal TotalAmount { get; set; }
    public decimal SecurityDeposit { get; set; }      // charged separately, not taxed
    public decimal AdvanceAmount { get; set; }
    public decimal LateGraceHours { get; set; }       // snapshot of grace period
    public decimal LatePenaltyMultiplier { get; set; } = 1.5m;
    public string? CouponCode { get; set; }

    // ---- Return settlement ----
    public decimal LateFee { get; set; }
    public decimal DamageCharge { get; set; }

    // ---- Cancellation ----
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }

    public ICollection<BookingModification> Modifications { get; set; } = new List<BookingModification>();
    public ICollection<VehicleInspection> Inspections { get; set; } = new List<VehicleInspection>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public Invoice? Invoice { get; set; }

    // ---------------- State transitions ----------------

    public void Confirm()
    {
        EnsureStatus(BookingStatus.Pending, "confirm");
        Status = BookingStatus.Confirmed;
        PendingExpiresAt = null;
    }

    public void Expire()
    {
        EnsureStatus(BookingStatus.Pending, "expire");
        Status = BookingStatus.Expired;
    }

    public void MarkPickedUp(DateTime pickedUpAt)
    {
        EnsureStatus(BookingStatus.Confirmed, "pick up");
        ActualPickupAt = pickedUpAt;
        Status = BookingStatus.PickedUp;
    }

    public void MarkReturned(DateTime returnedAt, decimal damageCharge)
    {
        EnsureStatus(BookingStatus.PickedUp, "return");
        ActualReturnAt = returnedAt;
        DamageCharge = damageCharge;
        LateFee = CalculateLateFee(returnedAt);
        Status = BookingStatus.Returned;
    }

    public void Complete()
    {
        EnsureStatus(BookingStatus.Returned, "complete");
        Status = BookingStatus.Completed;
    }

    public void Cancel(string reason, DateTime now)
    {
        if (Status is not (BookingStatus.Pending or BookingStatus.Confirmed))
            throw new DomainException($"A booking in '{Status}' state cannot be cancelled.");

        Status = BookingStatus.Cancelled;
        CancelledAt = now;
        CancellationReason = reason;
    }

    // ---------------- Calculations ----------------

    /// late_hours = ceil((actualReturn - endAt) - grace); fee = hours * hourlyRate * multiplier
    public decimal CalculateLateFee(DateTime returnedAt)
    {
        var late = returnedAt - EndAt - TimeSpan.FromHours((double)LateGraceHours);
        if (late <= TimeSpan.Zero) return 0m;

        var lateHours = (decimal)Math.Ceiling(late.TotalHours);
        var hourlyRate = RateType == RateType.Hourly ? RateApplied : RateApplied / 24m;
        return Math.Round(lateHours * hourlyRate * LatePenaltyMultiplier, 2);
    }

    /// Deposit left after late and damage charges. Negative means the customer owes a balance.
    public decimal DepositBalance() => SecurityDeposit - LateFee - DamageCharge;

    /// Example policy: >24h before pickup = 100%, otherwise 50%. Make this configurable in the service.
    public decimal CalculateRefundableAdvance(DateTime now, decimal paidAdvance)
    {
        var hoursToPickup = (StartAt - now).TotalHours;
        return hoursToPickup > 24 ? paidAdvance : Math.Round(paidAdvance * 0.5m, 2);
    }

    private void EnsureStatus(BookingStatus expected, string action)
    {
        if (Status != expected)
            throw new DomainException($"Cannot {action} a booking in '{Status}' state. Expected '{expected}'.");
    }
}
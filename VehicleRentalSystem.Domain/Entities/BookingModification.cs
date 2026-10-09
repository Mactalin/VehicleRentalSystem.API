namespace VehicleRentalSystem.Domain.Entities;

public class BookingModification : BaseEntity
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public DateTime OldStartAt { get; set; }
    public DateTime OldEndAt { get; set; }
    public DateTime NewStartAt { get; set; }
    public DateTime NewEndAt { get; set; }

    public int? OldVehicleId { get; set; }
    public int? NewVehicleId { get; set; }

    public decimal PriceDifference { get; set; }   // positive = customer pays more, negative = refund
    public string? Reason { get; set; }
    public int ModifiedByUserId { get; set; }
}
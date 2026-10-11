

using VehicleRentalSystem.Domain.Common;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Entities;

/// One PICKUP and one RETURN inspection per booking (enforce with a unique index on BookingId + Type).
public class VehicleInspection : BaseEntity
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public InspectionType Type { get; set; }
    public int OdometerReading { get; set; }
    public decimal FuelLevelPercent { get; set; }      // 0 - 100
    public string? DamageNotes { get; set; }
    public string? PhotoUrls { get; set; }             // comma separated or JSON
    public decimal DamageCharge { get; set; }          // only meaningful for RETURN
    public DateTime InspectedAt { get; set; } = DateTime.UtcNow;
    public int InspectedByUserId { get; set; }
}
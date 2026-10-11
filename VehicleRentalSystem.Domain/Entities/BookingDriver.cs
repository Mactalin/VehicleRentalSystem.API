using VehicleRentalSystem.Domain.Common;

namespace VehicleRentalSystem.Domain.Entities
{
    public class BookingDriver : BaseEntity
    {
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

        public int DriverId { get; set; }
        public Driver Driver { get; set; } = null!;

        public decimal ChargeApplied { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReleasedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
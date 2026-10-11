using VehicleRentalSystem.Domain.Common;

namespace VehicleRentalSystem.Domain.Entities
{
    public class CouponUsage : BaseEntity
    {
        public int CouponId { get; set; }
        public Coupon Coupon { get; set; } = null!;

        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

        public int CustomerId { get; set; }
        public decimal DiscountApplied { get; set; }
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;
    }
}
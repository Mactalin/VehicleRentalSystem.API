using VehicleRentalSystem.Domain.Common;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Entities
{
    public class Coupon : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal MinOrderAmount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int? MaxUses { get; set; }
        public int UsedCount { get; set; }
        public int? MaxUsesPerCustomer { get; set; }
        public CouponStatus Status { get; set; } = CouponStatus.Active;
        public bool IsActive { get; set; } = true;

        public ICollection<CouponUsage> Usages { get; set; } = new List<CouponUsage>();

        public bool IsValidNow()
        {
            var now = DateTime.UtcNow;
            return IsActive
                && !IsDeleted
                && Status == CouponStatus.Active
                && now >= ValidFrom
                && now <= ValidTo
                && (!MaxUses.HasValue || UsedCount < MaxUses.Value);
        }

        public decimal CalculateDiscount(decimal orderAmount)
        {
            if (orderAmount < MinOrderAmount) return 0m;

            var discount = DiscountType == DiscountType.Percentage
                ? Math.Round(orderAmount * (DiscountValue / 100m), 2)
                : DiscountValue;

            if (MaxDiscountAmount.HasValue && discount > MaxDiscountAmount.Value)
                discount = MaxDiscountAmount.Value;

            return Math.Min(discount, orderAmount);
        }
    }
}
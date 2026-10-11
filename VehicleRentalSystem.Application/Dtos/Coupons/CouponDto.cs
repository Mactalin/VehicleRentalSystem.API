using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.DTOs.Coupons;

public class CouponDto
{
    public int Id { get; set; }
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
    public CouponStatus Status { get; set; }
    public bool IsActive { get; set; }
}
using System.ComponentModel.DataAnnotations;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.DTOs.Coupons;

public class CreateCouponDto
{
    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public DiscountType DiscountType { get; set; }

    [Range(0.01, 1_000_000)]
    public decimal DiscountValue { get; set; }

    public decimal? MaxDiscountAmount { get; set; }

    [Range(0, 1_000_000)]
    public decimal MinOrderAmount { get; set; }

    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }

    public int? MaxUses { get; set; }
    public int? MaxUsesPerCustomer { get; set; }
}
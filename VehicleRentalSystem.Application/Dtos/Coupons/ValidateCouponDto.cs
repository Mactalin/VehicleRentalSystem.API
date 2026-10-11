using System.ComponentModel.DataAnnotations;

namespace VehicleRentalSystem.Application.DTOs.Coupons;

public class ValidateCouponDto
{
    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Range(0, 10_000_000)]
    public decimal OrderAmount { get; set; }

    public int? CustomerId { get; set; }
}
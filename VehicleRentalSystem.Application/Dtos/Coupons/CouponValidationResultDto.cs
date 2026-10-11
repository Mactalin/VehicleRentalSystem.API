namespace VehicleRentalSystem.Application.DTOs.Coupons;

public class CouponValidationResultDto
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public int? CouponId { get; set; }
    public string? Code { get; set; }
}
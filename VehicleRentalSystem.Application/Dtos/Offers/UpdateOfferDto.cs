using System.ComponentModel.DataAnnotations;

namespace VehicleRentalSystem.Application.DTOs.Offers;

public class UpdateOfferDto
{
    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Range(0.01, 100)]
    public decimal DiscountPercentage { get; set; }

    public decimal? MaxDiscountAmount { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }

    [MaxLength(500)]
    public string? BannerImageUrl { get; set; }
}
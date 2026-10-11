using System.ComponentModel.DataAnnotations;

namespace VehicleRentalSystem.Application.DTOs.Reviews;

public class CreateReviewDto
{
    [Required]
    public int BookingId { get; set; }

    [Required]
    public int CustomerId { get; set; }

    public int? VehicleId { get; set; }
    public int? DriverId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }
}
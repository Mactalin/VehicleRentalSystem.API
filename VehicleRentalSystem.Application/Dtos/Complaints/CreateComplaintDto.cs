using System.ComponentModel.DataAnnotations;

namespace VehicleRentalSystem.Application.DTOs.Complaints;

public class CreateComplaintDto
{
    [Required]
    public int CustomerId { get; set; }

    public int? BookingId { get; set; }

    [Required, MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required, MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
}
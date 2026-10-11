using System.ComponentModel.DataAnnotations;

namespace VehicleRentalSystem.Application.DTOs.Complaints;

public class RespondToComplaintDto
{
    [Required, MaxLength(2000)]
    public string AdminResponse { get; set; } = string.Empty;

    [Required]
    public int AdminUserId { get; set; }
}
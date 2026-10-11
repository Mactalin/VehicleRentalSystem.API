using System.ComponentModel.DataAnnotations;

namespace VehicleRentalSystem.Application.DTOs.Drivers;

public class CreateDriverDto
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress, MaxLength(100)]
    public string? Email { get; set; }

    public DateOnly? LicenseExpiryDate { get; set; }
    public int ExperienceYears { get; set; }

    [Range(0, 1_000_000)]
    public decimal ChargePerDay { get; set; }

    [Range(0, 1_000_000)]
    public decimal ChargePerHour { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
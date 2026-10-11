using System.ComponentModel.DataAnnotations;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.DTOs.Drivers;

public class UpdateDriverDto
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

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

    public DriverStatus Status { get; set; }
    public bool IsActive { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.DTOs.Drivers;

public class DriverDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateOnly? LicenseExpiryDate { get; set; }
    public int ExperienceYears { get; set; }
    public decimal ChargePerDay { get; set; }
    public decimal ChargePerHour { get; set; }
    public DriverStatus Status { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}
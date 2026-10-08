using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.DTOs;

public class DriverDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsAvailable { get; set; }
    public decimal ChargePerDay { get; set; }
}

public class CreateDriverDto
{
    public string FullName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public decimal ChargePerDay { get; set; }
}

public class UpdateDriverDto
{
    public string FullName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsAvailable { get; set; }
    public decimal ChargePerDay { get; set; }
}
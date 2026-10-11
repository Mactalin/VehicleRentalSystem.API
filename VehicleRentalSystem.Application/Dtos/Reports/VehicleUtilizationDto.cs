namespace VehicleRentalSystem.Application.DTOs.Reports;

public class VehicleUtilizationDto
{
    public int VehicleId { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string VehicleName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public decimal UtilizationRate { get; set; }
}
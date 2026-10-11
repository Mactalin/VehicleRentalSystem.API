namespace VehicleRentalSystem.Application.DTOs.Reports;

public class BranchPerformanceDto
{
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public int VehiclesAvailable { get; set; }
}
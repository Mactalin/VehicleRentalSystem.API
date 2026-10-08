using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.DTOs.Reports;

public class MaintenanceReportDto
{
    public int TotalMaintenanceRecords { get; set; }
    public decimal TotalExpense { get; set; }
    public List<MaintenanceExpenseDto> ByVehicle { get; set; } = new();
}

public class MaintenanceExpenseDto
{
    public int VehicleId { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public decimal TotalCost { get; set; }
}
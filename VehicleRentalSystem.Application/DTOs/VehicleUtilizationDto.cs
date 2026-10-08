using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.DTOs.Reports;

public class VehicleUtilizationDto
{
    public int VehicleId { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public decimal UtilizationRate { get; set; } // %
}
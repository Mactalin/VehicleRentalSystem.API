using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.DTOs;

public class DashboardStatsDto
{
    public int TotalVehicles { get; set; }
    public int AvailableVehicles { get; set; }
    public int ActiveRentals { get; set; }
    public int TotalCustomers { get; set; }
    public int TodayBookings { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal PendingPayments { get; set; }
    public int VehiclesUnderMaintenance { get; set; }
}
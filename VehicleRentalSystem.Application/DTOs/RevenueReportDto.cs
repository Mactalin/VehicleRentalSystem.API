using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.DTOs.Reports;

public class RevenueReportDto
{
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal WeeklyRevenue { get; set; }
    public decimal DailyRevenue { get; set; }
    public List<RevenueDataPointDto> DataPoints { get; set; } = new();
}

public class RevenueDataPointDto
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public int BookingCount { get; set; }
}

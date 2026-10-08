using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.DTOs.Reports;

public class BookingReportDto
{
    public int TotalBookings { get; set; }
    public int CompletedBookings { get; set; }
    public int ActiveBookings { get; set; }
    public int CancelledBookings { get; set; }
    public List<BookingStatusCountDto> ByStatus { get; set; } = new();
}

public class BookingStatusCountDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.DTOs.Reports;

public class CustomerReportDto
{
    public int TotalCustomers { get; set; }
    public int NewCustomersThisMonth { get; set; }
    public int ActiveCustomers { get; set; }
    public List<TopCustomerDto> TopCustomers { get; set; } = new();
}

public class TopCustomerDto
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int BookingCount { get; set; }
    public decimal TotalSpent { get; set; }
}

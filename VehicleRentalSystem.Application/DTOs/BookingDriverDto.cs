using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.DTOs;

public class BookingDriverDto
{
    public int BookingId { get; set; }
    public int DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public decimal DriverCharge { get; set; }
}

public class AssignDriverDto
{
    public int BookingId { get; set; }
    public int DriverId { get; set; }
    public decimal DriverCharge { get; set; }
}
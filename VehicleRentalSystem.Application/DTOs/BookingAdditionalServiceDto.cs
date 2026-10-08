using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.DTOs;

public class BookingAdditionalServiceDto
{
    public int BookingId { get; set; }
    public int AdditionalServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class AddServiceToBookingDto
{
    public int BookingId { get; set; }
    public int AdditionalServiceId { get; set; }
}
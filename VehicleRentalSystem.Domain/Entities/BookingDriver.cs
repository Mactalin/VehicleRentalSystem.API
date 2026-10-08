using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleRentalSystem.Domain.Entities;

public class BookingDriver
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public int DriverId { get; set; }
    public Driver Driver { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal DriverCharge { get; set; }
}
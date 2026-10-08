using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Domain.Entities;

// STUB — replace with real entity from Module 7 (Mac)
public class Booking
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
}

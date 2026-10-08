using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Domain.Entities;

// STUB — replace with real entity from Module 2 (Anbu)
public class Vehicle
{
    public int Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
}
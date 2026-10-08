using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Domain.Entities;

// STUB — replace with real entity from Module 1 (Anbu)
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer";
}
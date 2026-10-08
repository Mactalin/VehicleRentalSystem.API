using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleRentalSystem.Domain.Entities;

public class Driver
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    [MaxLength(15)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    public bool IsAvailable { get; set; } = true;

    [Column(TypeName = "decimal(18,2)")]
    public decimal ChargePerDay { get; set; }

    // Navigation properties (assumed existing entities)
    public ICollection<BookingDriver>? BookingDrivers { get; set; }
}
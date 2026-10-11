using System.ComponentModel.DataAnnotations;

namespace VehicleRentalSystem.Application.DTOs.BookingDrivers;

public class AssignDriverDto
{
    [Required]
    public int BookingId { get; set; }

    [Required]
    public int DriverId { get; set; }

    [Range(0, 1_000_000)]
    public decimal ChargeApplied { get; set; }
}
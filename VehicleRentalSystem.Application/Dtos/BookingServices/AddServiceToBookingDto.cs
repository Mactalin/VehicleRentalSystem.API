using System.ComponentModel.DataAnnotations;

namespace VehicleRentalSystem.Application.DTOs.BookingServices;

public class AddServiceToBookingDto
{
    [Required]
    public int BookingId { get; set; }

    [Required]
    public int AdditionalServiceId { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; } = 1;
}
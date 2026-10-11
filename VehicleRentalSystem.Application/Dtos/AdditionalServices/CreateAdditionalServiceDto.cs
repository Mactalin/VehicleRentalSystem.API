using System.ComponentModel.DataAnnotations;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.DTOs.AdditionalServices;

public class CreateAdditionalServiceDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Range(0, 1_000_000)]
    public decimal Price { get; set; }

    public ServiceType Type { get; set; }
    public int? MaxQuantityPerBooking { get; set; }
}
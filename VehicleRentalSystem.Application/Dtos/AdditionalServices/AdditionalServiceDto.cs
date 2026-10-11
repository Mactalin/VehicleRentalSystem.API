using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.DTOs.AdditionalServices;

public class AdditionalServiceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public ServiceType Type { get; set; }
    public bool IsActive { get; set; }
    public int? MaxQuantityPerBooking { get; set; }
}
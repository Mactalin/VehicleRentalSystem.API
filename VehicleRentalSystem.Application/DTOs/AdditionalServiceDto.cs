using System;
using System.Collections.Generic;
using System.Text;

using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.DTOs;

public class AdditionalServiceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public ServiceType Type { get; set; }
}

public class CreateAdditionalServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public ServiceType Type { get; set; }
}
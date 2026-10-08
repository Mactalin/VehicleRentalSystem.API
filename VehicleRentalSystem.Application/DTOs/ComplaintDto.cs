using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.DTOs;

public class ComplaintDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int? BookingId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ComplaintStatus Status { get; set; }
    public string? AdminResponse { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class CreateComplaintDto
{
    public int CustomerId { get; set; }
    public int? BookingId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateComplaintStatusDto
{
    public ComplaintStatus Status { get; set; }
    public string? AdminResponse { get; set; }
}
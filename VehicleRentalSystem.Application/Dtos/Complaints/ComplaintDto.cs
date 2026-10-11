using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.DTOs.Complaints;

public class ComplaintDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int? BookingId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ComplaintStatus Status { get; set; }
    public string? AdminResponse { get; set; }
    public int? HandledByUserId { get; set; }
    public DateTime? RespondedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
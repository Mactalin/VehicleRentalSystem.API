using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.DTOs.Notifications;

public class NotificationRequest
{
    public int UserId { get; set; }
    public string ToEmail { get; set; } = string.Empty;
    public string ToPhone { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public NotificationEvent Event { get; set; }
    public bool SendEmail { get; set; } = true;
    public bool SendSms { get; set; }
    public string? ActionUrl { get; set; }
    public string? ActionLabel { get; set; }
}
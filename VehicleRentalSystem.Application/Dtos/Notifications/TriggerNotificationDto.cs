using System.ComponentModel.DataAnnotations;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.DTOs.Notifications;

public class TriggerNotificationDto
{
    [Required]
    public int UserId { get; set; }

    [EmailAddress]
    public string? ToEmail { get; set; }

    public string? ToPhone { get; set; }

    public NotificationEvent Event { get; set; }

    [MaxLength(2000)]
    public string? CustomMessage { get; set; }

    public string? ActionUrl { get; set; }
    public string? ActionLabel { get; set; }
}
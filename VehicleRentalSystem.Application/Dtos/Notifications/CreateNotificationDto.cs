using System.ComponentModel.DataAnnotations;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.DTOs.Notifications;

public class CreateNotificationDto
{
    [Required]
    public int UserId { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(2000)]
    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; }
    public NotificationEvent Event { get; set; }
}
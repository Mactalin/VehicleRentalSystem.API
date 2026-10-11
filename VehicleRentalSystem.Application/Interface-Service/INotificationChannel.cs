using VehicleRentalSystem.Application.DTOs.Notifications;

namespace VehicleRentalSystem.Application.Interface_Service;

public interface INotificationChannel
{
    Task SendAsync(NotificationRequest request, CancellationToken ct = default);
}
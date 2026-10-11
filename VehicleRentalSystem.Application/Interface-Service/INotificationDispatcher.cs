using VehicleRentalSystem.Application.DTOs.Notifications;

namespace VehicleRentalSystem.Application.Interface_Service;

public interface INotificationDispatcher
{
    Task DispatchAsync(NotificationRequest request, CancellationToken ct = default);
}
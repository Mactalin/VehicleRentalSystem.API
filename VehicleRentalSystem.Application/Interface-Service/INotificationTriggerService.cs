using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Interface_Service;

public interface INotificationTriggerService
{
    Task TriggerAsync(
        int userId,
        string? toEmail,
        string? toPhone,
        NotificationEvent evt,
        string? customMessage = null,
        string? actionUrl = null,
        string? actionLabel = null,
        CancellationToken ct = default);
}
using VehicleRentalSystem.Application.DTOs.Notifications;

namespace VehicleRentalSystem.Application.Interface_Service;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetAllAsync(CancellationToken ct = default);
    Task<NotificationDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<IEnumerable<NotificationDto>> GetUnreadByUserIdAsync(int userId, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(int userId, CancellationToken ct = default);
    Task<NotificationDto> CreateAsync(CreateNotificationDto dto, CancellationToken ct = default);
    Task MarkAsReadAsync(int id, CancellationToken ct = default);
    Task MarkAllAsReadAsync(int userId, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
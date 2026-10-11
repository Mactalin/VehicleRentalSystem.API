using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Interface_Repository
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId, CancellationToken ct = default);
        Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId, CancellationToken ct = default);
        Task<int> GetUnreadCountAsync(int userId, CancellationToken ct = default);
    }
}
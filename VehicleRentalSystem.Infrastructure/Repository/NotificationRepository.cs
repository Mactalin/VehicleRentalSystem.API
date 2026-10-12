using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId, CancellationToken ct = default)
        => await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId, CancellationToken ct = default)
        => await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);

    public async Task<int> GetUnreadCountAsync(int userId, CancellationToken ct = default)
        => await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead, ct);
}
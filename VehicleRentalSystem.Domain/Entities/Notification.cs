using VehicleRentalSystem.Domain.Common;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; }
        public NotificationEvent Event { get; set; }

        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime? SentAt { get; set; }
        public string? FailureReason { get; set; }

        public void MarkRead()
        {
            if (IsRead) return;
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }
}
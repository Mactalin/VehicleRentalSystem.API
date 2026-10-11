using VehicleRentalSystem.Domain.Common;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Exceptions;

namespace VehicleRentalSystem.Domain.Entities
{
    public class Complaint : BaseEntity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public int? BookingId { get; set; }
        public Booking? Booking { get; set; }

        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;

        public string? AdminResponse { get; set; }
        public int? HandledByUserId { get; set; }
        public DateTime? RespondedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public void Respond(string adminResponse, int adminUserId)
        {
            if (string.IsNullOrWhiteSpace(adminResponse))
                throw new DomainException("Admin response cannot be empty.");

            AdminResponse = adminResponse;
            HandledByUserId = adminUserId;
            RespondedAt = DateTime.UtcNow;
            Status = ComplaintStatus.InProgress;
        }

        public void Resolve()
        {
            if (Status == ComplaintStatus.Closed)
                throw new DomainException("A closed complaint cannot be resolved again.");

            Status = ComplaintStatus.Resolved;
            ResolvedAt = DateTime.UtcNow;
        }

        public void Close()
        {
            Status = ComplaintStatus.Closed;
            if (!ResolvedAt.HasValue) ResolvedAt = DateTime.UtcNow;
        }
    }
}
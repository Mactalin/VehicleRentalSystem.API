using VehicleRentalSystem.Domain.Common;
using VehicleRentalSystem.Domain.Exceptions;

namespace VehicleRentalSystem.Domain.Entities
{
    public class Review : BaseEntity
    {
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public int? VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        public int? DriverId { get; set; }
        public Driver? Driver { get; set; }

        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsVerifiedBooking { get; set; } = true;
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;

        public void ValidateRating()
        {
            if (Rating < 1 || Rating > 5)
                throw new DomainException("Rating must be between 1 and 5.");
        }
    }
}
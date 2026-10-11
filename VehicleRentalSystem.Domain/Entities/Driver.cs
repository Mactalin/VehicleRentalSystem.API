using VehicleRentalSystem.Domain.Common;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Entities
{
    public class Driver : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateOnly? LicenseExpiryDate { get; set; }
        public int ExperienceYears { get; set; }
        public decimal ChargePerDay { get; set; }
        public decimal ChargePerHour { get; set; }
        public DriverStatus Status { get; set; } = DriverStatus.Available;
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }

        public ICollection<BookingDriver> BookingAssignments { get; set; } = new List<BookingDriver>();

        public bool IsAvailable() => Status == DriverStatus.Available && IsActive && !IsDeleted;
    }
}
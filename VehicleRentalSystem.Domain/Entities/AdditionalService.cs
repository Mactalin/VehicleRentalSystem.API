using VehicleRentalSystem.Domain.Common;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Entities
{
    public class AdditionalService : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public ServiceType Type { get; set; }
        public bool IsActive { get; set; } = true;
        public int? MaxQuantityPerBooking { get; set; }

        public ICollection<BookingAdditionalService> BookingAssignments { get; set; } = new List<BookingAdditionalService>();
    }
}
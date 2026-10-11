using VehicleRentalSystem.Domain.Common;

namespace VehicleRentalSystem.Domain.Entities
{
    public class BookingAdditionalService : BaseEntity
    {
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

        public int AdditionalServiceId { get; set; }
        public AdditionalService AdditionalService { get; set; } = null!;

        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
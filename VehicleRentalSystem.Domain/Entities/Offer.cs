using VehicleRentalSystem.Domain.Common;

namespace VehicleRentalSystem.Domain.Entities
{
    public class Offer : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? BannerImageUrl { get; set; }

        public bool IsRunningNow()
        {
            var now = DateTime.UtcNow;
            return IsActive && !IsDeleted && now >= StartDate && now <= EndDate;
        }
    }
}
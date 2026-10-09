using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Customers
{
    public class CustomerFilterDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public VerificationStatus? Verification { get; set; }
        public CustomerStatus? Status { get; set; }
        public string? Search { get; set; }
    }
}

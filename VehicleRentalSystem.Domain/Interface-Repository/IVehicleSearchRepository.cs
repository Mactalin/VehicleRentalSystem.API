using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Interface_Repository;

public class VehicleSearchCriteria
{
    public int? BranchId { get; set; }
    public string? City { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }          // compared with the category's daily price
    public decimal? MaxPrice { get; set; }
    public FuelType? FuelType { get; set; }
    public TransmissionType? Transmission { get; set; }
    public int? MinSeats { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public interface IVehicleSearchRepository
{
    /// Returns vehicles with Category (+Pricing), Branch and Images loaded.
    Task<(IReadOnlyList<Vehicle> Items, int Total)> SearchAvailableAsync(
        VehicleSearchCriteria criteria, CancellationToken ct = default);
}
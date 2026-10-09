using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Interface_Repository;

public class VehicleSearchCriteria
{
    public int? BranchId { get; set; }
    public string? City { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public IReadOnlyList<int>? FeatureIds { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// Anbu owns the Vehicle entity; Mac only needs the search + availability query.
public interface IVehicleRepository : IGenericRepository<Vehicle>
{
    Task<(IReadOnlyList<Vehicle> Items, int Total)> SearchAvailableAsync(
        VehicleSearchCriteria criteria, CancellationToken ct = default);
}
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Interface_Repository;

public interface IBranchLookupRepository
{
    Task<bool> BranchExistsAsync(int branchId, CancellationToken ct = default);
    Task<BranchOperatingHour?> GetOperatingHourAsync(int branchId, DayOfWeek day, CancellationToken ct = default);
}
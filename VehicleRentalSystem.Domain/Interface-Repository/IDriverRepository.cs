using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Interface_Repository
{
    public interface IDriverRepository : IGenericRepository<Driver>
    {
        Task<IEnumerable<Driver>> GetAvailableAsync(CancellationToken ct = default);
        Task<IEnumerable<Driver>> GetByStatusAsync(DriverStatus status, CancellationToken ct = default);
        Task<Driver?> GetByLicenseNumberAsync(string licenseNumber, CancellationToken ct = default);
        Task<bool> HasActiveAssignmentAsync(int driverId, CancellationToken ct = default);
    }
}
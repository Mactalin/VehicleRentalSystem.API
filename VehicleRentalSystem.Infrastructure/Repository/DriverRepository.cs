using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository;

public class DriverRepository : GenericRepository<Driver>, IDriverRepository
{
    public DriverRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<Driver>> GetAvailableAsync(CancellationToken ct = default)
        => await _db.Drivers
            .Where(d => d.Status == DriverStatus.Available && d.IsActive)
            .OrderBy(d => d.FullName)
            .ToListAsync(ct);

    public async Task<IEnumerable<Driver>> GetByStatusAsync(DriverStatus status, CancellationToken ct = default)
        => await _db.Drivers
            .Where(d => d.Status == status)
            .OrderBy(d => d.FullName)
            .ToListAsync(ct);

    public async Task<Driver?> GetByLicenseNumberAsync(string licenseNumber, CancellationToken ct = default)
        => await _db.Drivers.FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber, ct);

    public async Task<bool> HasActiveAssignmentAsync(int driverId, CancellationToken ct = default)
        => await _db.BookingDrivers
            .AnyAsync(bd => bd.DriverId == driverId && bd.IsActive, ct);
}
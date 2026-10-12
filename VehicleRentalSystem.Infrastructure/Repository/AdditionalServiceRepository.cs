using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository;

public class AdditionalServiceRepository : GenericRepository<AdditionalService>, IAdditionalServiceRepository
{
    public AdditionalServiceRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<AdditionalService>> GetActiveAsync(CancellationToken ct = default)
        => await _context.AdditionalServices
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(ct);

    public async Task<IEnumerable<AdditionalService>> GetByTypeAsync(ServiceType type, CancellationToken ct = default)
        => await _context.AdditionalServices
            .Where(s => s.Type == type && s.IsActive)
            .ToListAsync(ct);
}
using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository;

public class OfferRepository : GenericRepository<Offer>, IOfferRepository
{
    public OfferRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<Offer>> GetRunningAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Offers
            .Where(o => o.IsActive && o.StartDate <= now && o.EndDate >= now)
            .OrderBy(o => o.EndDate)
            .ToListAsync(ct);
    }
}
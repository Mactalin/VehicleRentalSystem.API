using Microsoft.EntityFrameworkCore;

using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository
{
    public class KycRepository : GenericRepository<KycDocument>, IKycRepository
    {
        public KycRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<KycDocument>> GetByCustomerAsync(int customerId, CancellationToken ct = default) =>
            await _dbSet.AsNoTracking()
                .Where(k => k.CustomerId == customerId)
                .OrderByDescending(k => k.CreatedAt)
                .ToListAsync(ct);
    }
}

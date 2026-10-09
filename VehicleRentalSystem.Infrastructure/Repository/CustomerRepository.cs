using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;

using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context) { }

    public async Task<Customer?> GetByUserIdAsync(int userId, CancellationToken ct = default) =>
        await _dbSet.FirstOrDefaultAsync(c => c.UserId == userId, ct);

    public async Task<Customer?> GetWithDocumentsAsync(int customerId, CancellationToken ct = default) =>
        await _dbSet
            .Include(c => c.KycDocuments)
            .Include(c => c.DrivingLicences)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == customerId, ct);

    public async Task<bool> LicenceNumberExistsAsync(string licenceNumber, CancellationToken ct = default) =>
        await _context.DrivingLicences.AnyAsync(d => d.LicenceNumber == licenceNumber, ct);

    public async Task<(IReadOnlyList<Customer> Items, int Total)> GetPagedAsync(
        int page, int pageSize, VerificationStatus? verification, CustomerStatus? status,
        string? search, CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _dbSet.AsNoTracking().AsQueryable();

        if (verification.HasValue) query = query.Where(c => c.VerificationStatus == verification.Value);
        if (status.HasValue) query = query.Where(c => c.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c => c.FullName.Contains(term) || c.Phone.Contains(term));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }
}
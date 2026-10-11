using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository;

public class ComplaintRepository : GenericRepository<Complaint>, IComplaintRepository
{
    public ComplaintRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<Complaint>> GetByStatusAsync(ComplaintStatus status, CancellationToken ct = default)
        => await _db.Complaints
            .Where(c => c.Status == status)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Complaint>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default)
        => await _db.Complaints
            .Where(c => c.CustomerId == customerId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);
}
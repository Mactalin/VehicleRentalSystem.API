using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository;

public class BranchLookupRepository : IBranchLookupRepository
{
    private readonly AppDbContext _context;

    public BranchLookupRepository(AppDbContext context) => _context = context;

    public async Task<bool> BranchExistsAsync(int branchId, CancellationToken ct = default) =>
        await _context.Branches.AnyAsync(b => b.Id == branchId && b.IsActive, ct);

    public async Task<BranchOperatingHour?> GetOperatingHourAsync(
        int branchId, DayOfWeek day, CancellationToken ct = default) =>
        await _context.BranchOperatingHours
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.BranchId == branchId && h.DayOfWeek == day, ct);
}
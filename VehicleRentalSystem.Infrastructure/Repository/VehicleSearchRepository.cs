using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository;

public class VehicleSearchRepository : IVehicleSearchRepository
{
    private readonly AppDbContext _context;

    public VehicleSearchRepository(AppDbContext context) => _context = context;

    public async Task<(IReadOnlyList<Vehicle> Items, int Total)> SearchAvailableAsync(
        VehicleSearchCriteria c, CancellationToken ct = default)
    {
        var page = Math.Max(1, c.Page);
        var pageSize = Math.Clamp(c.PageSize, 1, 100);

        var query = _context.Vehicles.AsNoTracking()
            .Where(v => v.IsActive
                        && v.Status != VehicleStatus.Maintenance
                        && v.Category.IsActive
                        && v.Branch.IsActive);

        if (c.BranchId.HasValue)
            query = query.Where(v => v.BranchId == c.BranchId.Value);

        if (!string.IsNullOrWhiteSpace(c.City))
        {
            var city = c.City.Trim();
            query = query.Where(v => v.Branch.City == city);
        }

        if (c.CategoryId.HasValue)
            query = query.Where(v => v.CategoryId == c.CategoryId.Value);

        if (c.MinPrice.HasValue)
            query = query.Where(v => v.Category.Pricing != null && v.Category.Pricing.DailyPrice >= c.MinPrice.Value);

        if (c.MaxPrice.HasValue)
            query = query.Where(v => v.Category.Pricing != null && v.Category.Pricing.DailyPrice <= c.MaxPrice.Value);

        if (c.FuelType.HasValue)
            query = query.Where(v => v.FuelType == c.FuelType.Value);

        if (c.Transmission.HasValue)
            query = query.Where(v => v.Transmission == c.Transmission.Value);

        if (c.MinSeats.HasValue)
            query = query.Where(v => v.SeatingCapacity >= c.MinSeats.Value);

        // Real-time availability: no blocking booking and no maintenance in the range
        if (c.From.HasValue && c.To.HasValue)
        {
            var from = c.From.Value;
            var to = c.To.Value;
            var now = DateTime.UtcNow;
            var fromDate = DateOnly.FromDateTime(from);
            var toDate = DateOnly.FromDateTime(to);

            query = query.Where(v =>
                !_context.Bookings.Any(b =>
                    b.VehicleId == v.Id
                    && b.StartAt < to
                    && b.EndAt > from
                    && (b.Status == BookingStatus.Confirmed
                        || b.Status == BookingStatus.PickedUp
                        || (b.Status == BookingStatus.Pending && b.PendingExpiresAt > now)))
                && !_context.MaintenanceRecords.Any(m =>
                    m.VehicleId == v.Id
                    && ((m.Status == MaintenanceStatus.InProgress && m.ScheduledDate <= toDate)
                        || (m.Status == MaintenanceStatus.Scheduled
                            && m.ScheduledDate >= fromDate && m.ScheduledDate <= toDate))));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .Include(v => v.Category).ThenInclude(cat => cat.Pricing)
            .Include(v => v.Branch)
            .Include(v => v.Images)
            .OrderBy(v => v.Category.Pricing!.DailyPrice)
            .ThenBy(v => v.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsSplitQuery()
            .ToListAsync(ct);

        return (items, total);
    }
}
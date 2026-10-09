using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository
{

    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(AppDbContext context) : base(context) { }

        public async Task<(IReadOnlyList<Vehicle> Items, int Total)> SearchAvailableAsync(
            VehicleSearchCriteria criteria, CancellationToken ct = default)
        {
            var page = Math.Max(1, criteria.Page);
            var pageSize = Math.Clamp(criteria.PageSize, 1, 100);

            var query = _dbSet.AsNoTracking().Where(v => v.IsActive);

            // Location
            if (criteria.BranchId.HasValue)
                query = query.Where(v => v.BranchId == criteria.BranchId.Value);

            if (!string.IsNullOrWhiteSpace(criteria.City))
            {
                var city = criteria.City.Trim();
                query = query.Where(v => _context.Branches.Any(b => b.Id == v.BranchId && b.City == city));
            }

            // Category and price
            if (criteria.CategoryId.HasValue)
                query = query.Where(v => v.CategoryId == criteria.CategoryId.Value);

            if (criteria.MinPrice.HasValue)
                query = query.Where(v => v.DailyRate >= criteria.MinPrice.Value);

            if (criteria.MaxPrice.HasValue)
                query = query.Where(v => v.DailyRate <= criteria.MaxPrice.Value);

            // Real-time availability for the requested range
            if (criteria.From.HasValue && criteria.To.HasValue)
            {
                var from = criteria.From.Value;
                var to = criteria.To.Value;
                var now = DateTime.UtcNow;

                query = query.Where(v => !_context.Bookings.Any(b =>
                    b.VehicleId == v.Id
                    && b.StartAt < to
                    && b.EndAt > from
                    && (b.Status == BookingStatus.Confirmed
                        || b.Status == BookingStatus.PickedUp
                        || (b.Status == BookingStatus.Pending && b.PendingExpiresAt > now))));

                // TODO (Anbu): also exclude vehicles with a maintenance block overlapping [from, to)
            }

            // TODO (Anbu): feature filter, e.g.
            // if (criteria.FeatureIds is { Count: > 0 })
            //     query = query.Where(v => criteria.FeatureIds.All(f => v.Features.Any(vf => vf.FeatureId == f)));

            var total = await query.CountAsync(ct);
            var items = await query
                .OrderBy(v => v.DailyRate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }
    }
}

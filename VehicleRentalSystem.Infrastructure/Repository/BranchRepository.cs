using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository
{
    public class BranchRepository : GenericRepository<Branch>, IBranchRepository
    {
        public BranchRepository(AppDbContext context) : base(context) { }

        public async Task<Branch?> GetByIdWithHoursAsync(int id) =>
            await _dbSet.Include(b => b.OperatingHours).FirstOrDefaultAsync(b => b.Id == id);

        public async Task<IEnumerable<Branch>> GetAllWithHoursAsync() =>
            await _dbSet.AsNoTracking().Include(b => b.OperatingHours).OrderBy(b => b.Name).ToListAsync();

        public async Task<IEnumerable<Branch>> GetByCityAsync(string city) =>
            await _dbSet.AsNoTracking().Include(b => b.OperatingHours)
                        .Where(b => b.City == city && b.IsActive).ToListAsync();

        public async Task<IEnumerable<BranchOperatingHour>> GetOperatingHoursAsync(int branchId) =>
            await _context.BranchOperatingHours
                .Where(h => h.BranchId == branchId)
                .OrderBy(h => h.DayOfWeek).ToListAsync();

        public async Task AddOperatingHoursAsync(IEnumerable<BranchOperatingHour> hours) =>
            await _context.BranchOperatingHours.AddRangeAsync(hours);

        public void RemoveOperatingHours(IEnumerable<BranchOperatingHour> hours) =>
            _context.BranchOperatingHours.RemoveRange(hours);
    }
}

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
    public class MaintenanceRepository : GenericRepository<MaintenanceRecord>, IMaintenanceRepository
    {
        public MaintenanceRepository(AppDbContext context) : base(context) { }

        public async Task<MaintenanceRecord?> GetByIdWithVehicleAsync(int id) =>
            await _dbSet.Include(m => m.Vehicle).FirstOrDefaultAsync(m => m.Id == id);

        public async Task<IEnumerable<MaintenanceRecord>> GetByVehicleAsync(int vehicleId) =>
            await _dbSet.AsNoTracking()
                .Where(m => m.VehicleId == vehicleId)
                .OrderByDescending(m => m.ScheduledDate).ToListAsync();

        public async Task<IEnumerable<MaintenanceRecord>> GetByStatusAsync(MaintenanceStatus status) =>
            await _dbSet.AsNoTracking().Include(m => m.Vehicle)
                .Where(m => m.Status == status)
                .OrderBy(m => m.ScheduledDate).ToListAsync();

        public async Task<IEnumerable<MaintenanceRecord>> GetUpcomingAsync(DateOnly from, DateOnly to) =>
            await _dbSet.AsNoTracking().Include(m => m.Vehicle)
                .Where(m => m.ScheduledDate >= from && m.ScheduledDate <= to
                            && (m.Status == MaintenanceStatus.Scheduled || m.Status == MaintenanceStatus.InProgress))
                .OrderBy(m => m.ScheduledDate).ToListAsync();

        public async Task<decimal> GetTotalCostByVehicleAsync(int vehicleId) =>
            await _dbSet.Where(m => m.VehicleId == vehicleId && m.Status == MaintenanceStatus.Completed)
                        .SumAsync(m => m.Cost);
    }
}

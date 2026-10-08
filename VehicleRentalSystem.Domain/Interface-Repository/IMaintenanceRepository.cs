using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Interface_Repository
{
    public interface IMaintenanceRepository : IGenericRepository<MaintenanceRecord>
    {
        Task<MaintenanceRecord?> GetByIdWithVehicleAsync(int id);
        Task<IEnumerable<MaintenanceRecord>> GetByVehicleAsync(int vehicleId);
        Task<IEnumerable<MaintenanceRecord>> GetByStatusAsync(MaintenanceStatus status);
        Task<IEnumerable<MaintenanceRecord>> GetUpcomingAsync(DateOnly from, DateOnly to);
        Task<decimal> GetTotalCostByVehicleAsync(int vehicleId);
    }
}

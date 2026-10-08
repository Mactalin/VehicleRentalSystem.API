using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Maintenance;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Interface_Service
{
    public interface IMaintenanceService
    {
        Task<MaintenanceDto> GetByIdAsync(int id);
        Task<IEnumerable<MaintenanceDto>> GetByVehicleAsync(int vehicleId);
        Task<IEnumerable<MaintenanceDto>> GetByStatusAsync(MaintenanceStatus status);
        Task<IEnumerable<MaintenanceDto>> GetUpcomingAsync(int withinDays = 30);
        Task<decimal> GetTotalCostAsync(int vehicleId);

        Task<MaintenanceDto> CreateAsync(SaveMaintenanceDto dto);
        Task<MaintenanceDto> UpdateAsync(int id, SaveMaintenanceDto dto);
        Task<MaintenanceDto> UpdateStatusAsync(int id, MaintenanceStatus status);
        Task<MaintenanceDto> CompleteAsync(int id, CompleteMaintenanceDto dto);
        Task DeleteAsync(int id);
    }
}

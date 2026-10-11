using VehicleRentalSystem.Application.DTOs.Dashboard;

namespace VehicleRentalSystem.Application.Interface_Service;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync(CancellationToken ct = default);
}
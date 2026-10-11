using VehicleRentalSystem.Application.DTOs.Dashboard;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IReportRepository _repo;
    public DashboardService(IReportRepository repo) => _repo = repo;

    public async Task<DashboardStatsDto> GetStatsAsync(CancellationToken ct = default) => new()
    {
        TotalVehicles = await _repo.GetTotalVehiclesAsync(ct),
        AvailableVehicles = await _repo.GetAvailableVehiclesAsync(ct),
        ActiveRentals = await _repo.GetActiveRentalsAsync(ct),
        TotalCustomers = await _repo.GetTotalCustomersAsync(ct),
        TodayBookings = await _repo.GetTodayBookingsAsync(ct),
        MonthlyRevenue = await _repo.GetMonthlyRevenueAsync(ct),
        PendingPayments = await _repo.GetPendingPaymentsAsync(ct),
        VehiclesUnderMaintenance = await _repo.GetVehiclesUnderMaintenanceAsync(ct)
    };
}
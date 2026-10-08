using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.DTOs;
using VehicleRentalSystem.Application.ServiceInterfaces;
using VehicleRentalSystem.Domain.RepositoryInterfaces;

namespace VehicleRentalSystem.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IReportRepository _reportRepository;

    public DashboardService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        return new DashboardStatsDto
        {
            TotalVehicles = await _reportRepository.GetTotalVehiclesAsync(),
            AvailableVehicles = await _reportRepository.GetAvailableVehiclesAsync(),
            ActiveRentals = await _reportRepository.GetActiveRentalsAsync(),
            TotalCustomers = await _reportRepository.GetTotalCustomersAsync(),
            TodayBookings = await _reportRepository.GetTodayBookingsAsync(),
            MonthlyRevenue = await _reportRepository.GetMonthlyRevenueAsync(),
            PendingPayments = await _reportRepository.GetPendingPaymentsAsync(),
            VehiclesUnderMaintenance = await _reportRepository.GetVehiclesUnderMaintenanceAsync()
        };
    }
}
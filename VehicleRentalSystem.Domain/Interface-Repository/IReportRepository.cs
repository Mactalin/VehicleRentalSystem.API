using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;
namespace VehicleRentalSystem.Domain.RepositoryInterfaces;


public interface IReportRepository
{
    Task<int> GetTotalVehiclesAsync();
    Task<int> GetAvailableVehiclesAsync();
    Task<int> GetActiveRentalsAsync();
    Task<int> GetTotalCustomersAsync();
    Task<int> GetTodayBookingsAsync();
    Task<decimal> GetMonthlyRevenueAsync();
    Task<decimal> GetPendingPaymentsAsync();
    Task<int> GetVehiclesUnderMaintenanceAsync();

    Task<IEnumerable<Booking>> GetBookingsInRangeAsync(DateTime from, DateTime to);
    Task<IEnumerable<Vehicle>> GetVehiclesAsync();
    Task<IEnumerable<Customer>> GetCustomersAsync();
}
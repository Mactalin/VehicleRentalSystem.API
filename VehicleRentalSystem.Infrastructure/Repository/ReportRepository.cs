using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Application.DTOs.Reports;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.RepositoryInterfaces;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;

    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetTotalVehiclesAsync()
        => await _context.Vehicles.CountAsync();

    public async Task<int> GetAvailableVehiclesAsync()
        => await _context.Vehicles.CountAsync(v => v.IsAvailable);

    public async Task<int> GetActiveRentalsAsync()
        => await _context.Bookings.CountAsync(b => b.Status == "Active");

    public async Task<int> GetTotalCustomersAsync()
        => await _context.Customers.CountAsync();

    public async Task<int> GetTodayBookingsAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Bookings.CountAsync(b => b.CreatedAt.Date == today);
    }

    public async Task<decimal> GetMonthlyRevenueAsync()
    {
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        return await _context.Bookings
            .Where(b => b.CreatedAt >= startOfMonth && b.Status == "Completed")
            .SumAsync(b => (decimal?)b.TotalAmount) ?? 0m;
    }

    public async Task<decimal> GetPendingPaymentsAsync()
    {
        // Placeholder until Payments module is implemented
        return await Task.FromResult(0m);
    }

    public async Task<int> GetVehiclesUnderMaintenanceAsync()
    {
        // Placeholder until Maintenance module is implemented
        return await Task.FromResult(0);
    }


public async Task<RevenueReportDto> GetRevenueReportAsync(DateTime? from, DateTime? to)
{
    from ??= DateTime.UtcNow.AddMonths(-1);
    to ??= DateTime.UtcNow;

    var bookings = await _context.Bookings
        .Where(b => b.CreatedAt >= from && b.CreatedAt <= to && b.Status == "Completed")
        .ToListAsync();

    var now = DateTime.UtcNow;
    return new RevenueReportDto
    {
        TotalRevenue = bookings.Sum(b => b.TotalAmount),
        MonthlyRevenue = bookings.Where(b => b.CreatedAt >= now.AddDays(-30)).Sum(b => b.TotalAmount),
        WeeklyRevenue = bookings.Where(b => b.CreatedAt >= now.AddDays(-7)).Sum(b => b.TotalAmount),
        DailyRevenue = bookings.Where(b => b.CreatedAt.Date == now.Date).Sum(b => b.TotalAmount),
        DataPoints = bookings
            .GroupBy(b => b.CreatedAt.Date)
            .Select(g => new RevenueDataPointDto
            {
                Date = g.Key,
                Amount = g.Sum(x => x.TotalAmount),
                BookingCount = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToList()
    };
}

public async Task<BookingReportDto> GetBookingReportAsync(DateTime? from, DateTime? to)
{
    from ??= DateTime.UtcNow.AddMonths(-1);
    to ??= DateTime.UtcNow;

    var bookings = await _context.Bookings
        .Where(b => b.CreatedAt >= from && b.CreatedAt <= to)
        .ToListAsync();

    return new BookingReportDto
    {
        TotalBookings = bookings.Count,
        CompletedBookings = bookings.Count(b => b.Status == "Completed"),
        ActiveBookings = bookings.Count(b => b.Status == "Active"),
        CancelledBookings = bookings.Count(b => b.Status == "Cancelled"),
        ByStatus = bookings
            .GroupBy(b => b.Status)
            .Select(g => new BookingStatusCountDto { Status = g.Key, Count = g.Count() })
            .ToList()
    };
}

public async Task<IEnumerable<VehicleUtilizationDto>> GetVehicleUtilizationAsync()
{
    var vehicles = await _context.Vehicles.ToListAsync();
    var totalBookings = await _context.Bookings.CountAsync();

    return vehicles.Select(v => new VehicleUtilizationDto
    {
        VehicleId = v.Id,
        RegistrationNumber = v.RegistrationNumber,
        TotalBookings = 0, // placeholder until Booking has VehicleId
        UtilizationRate = totalBookings == 0 ? 0 : 0m
    });
}

public async Task<CustomerReportDto> GetCustomerReportAsync()
{
    var customers = await _context.Customers.ToListAsync();
    var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

    return new CustomerReportDto
    {
        TotalCustomers = customers.Count,
        NewCustomersThisMonth = 0, // placeholder until Customer has CreatedAt
        ActiveCustomers = customers.Count,
        TopCustomers = new List<TopCustomerDto>()
    };
}

public Task<PaymentReportDto> GetPaymentReportAsync()
    => Task.FromResult(new PaymentReportDto());

public Task<MaintenanceReportDto> GetMaintenanceReportAsync()
    => Task.FromResult(new MaintenanceReportDto());

public Task<IEnumerable<BranchPerformanceDto>> GetBranchPerformanceAsync()
    => Task.FromResult(Enumerable.Empty<BranchPerformanceDto>());

    public async Task<IEnumerable<Booking>> GetBookingsInRangeAsync(DateTime from, DateTime to)
    => await _context.Bookings
        .Where(b => b.CreatedAt >= from && b.CreatedAt <= to)
        .ToListAsync();

    public async Task<IEnumerable<Vehicle>> GetVehiclesAsync()
        => await _context.Vehicles.ToListAsync();

    public async Task<IEnumerable<Customer>> GetCustomersAsync()
        => await _context.Customers.ToListAsync();
}

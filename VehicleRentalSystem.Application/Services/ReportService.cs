using VehicleRentalSystem.Application.DTOs.Reports;
using VehicleRentalSystem.Application.ServiceInterfaces;
using VehicleRentalSystem.Domain.RepositoryInterfaces;

namespace VehicleRentalSystem.Application.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _repo;

    public ReportService(IReportRepository repo) => _repo = repo;

    public async Task<RevenueReportDto> GetRevenueReportAsync(DateTime? from, DateTime? to)
    {
        from ??= DateTime.UtcNow.AddMonths(-1);
        to ??= DateTime.UtcNow;

        var bookings = (await _repo.GetBookingsInRangeAsync(from.Value, to.Value))
            .Where(b => b.Status == "Completed")
            .ToList();

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

        var bookings = (await _repo.GetBookingsInRangeAsync(from.Value, to.Value)).ToList();

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
        var vehicles = await _repo.GetVehiclesAsync();
        return vehicles.Select(v => new VehicleUtilizationDto
        {
            VehicleId = v.Id,
            RegistrationNumber = v.RegistrationNumber,
            TotalBookings = 0,
            UtilizationRate = 0m
        });
    }

    public async Task<CustomerReportDto> GetCustomerReportAsync()
    {
        var customers = (await _repo.GetCustomersAsync()).ToList();
        return new CustomerReportDto
        {
            TotalCustomers = customers.Count,
            NewCustomersThisMonth = 0,
            ActiveCustomers = customers.Count,
            TopCustomers = new List<TopCustomerDto>()
        };
    }

    public Task<PaymentReportDto> GetPaymentReportAsync() => Task.FromResult(new PaymentReportDto());
    public Task<MaintenanceReportDto> GetMaintenanceReportAsync() => Task.FromResult(new MaintenanceReportDto());
    public Task<IEnumerable<BranchPerformanceDto>> GetBranchPerformanceAsync()
        => Task.FromResult(Enumerable.Empty<BranchPerformanceDto>());
}
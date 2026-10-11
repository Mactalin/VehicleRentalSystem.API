using VehicleRentalSystem.Application.DTOs.Reports;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _repo;
    public ReportService(IReportRepository repo) => _repo = repo;

    public async Task<RevenueReportDto> GetRevenueReportAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        from ??= DateTime.UtcNow.AddMonths(-1);
        to ??= DateTime.UtcNow;

        var bookings = (await _repo.GetBookingsInRangeAsync(from.Value, to.Value, ct))
            .Where(b => b.Status == BookingStatus.Completed)
            .ToList();

        var now = DateTime.UtcNow;

        return new RevenueReportDto
        {
            TotalRevenue = bookings.Sum(b => b.TotalAmount),
            MonthlyRevenue = bookings.Where(b => b.StartAt >= now.AddDays(-30)).Sum(b => b.TotalAmount),
            WeeklyRevenue = bookings.Where(b => b.StartAt >= now.AddDays(-7)).Sum(b => b.TotalAmount),
            DailyRevenue = bookings.Where(b => b.StartAt.Date == now.Date).Sum(b => b.TotalAmount),
            TotalBookings = bookings.Count,
            DataPoints = bookings
                .GroupBy(b => b.StartAt.Date)
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

    public async Task<BookingReportDto> GetBookingReportAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        from ??= DateTime.UtcNow.AddMonths(-1);
        to ??= DateTime.UtcNow;

        var bookings = (await _repo.GetBookingsInRangeAsync(from.Value, to.Value, ct)).ToList();

        return new BookingReportDto
        {
            TotalBookings = bookings.Count,
            CompletedBookings = bookings.Count(b => b.Status == BookingStatus.Completed),
            ActiveBookings = bookings.Count(b =>
                b.Status == BookingStatus.Confirmed ||
                b.Status == BookingStatus.PickedUp),
            CancelledBookings = bookings.Count(b => b.Status == BookingStatus.Cancelled),
            ByStatus = bookings
                .GroupBy(b => b.Status.ToString())
                .Select(g => new BookingStatusCountDto { Status = g.Key, Count = g.Count() })
                .ToList()
        };
    }

    public async Task<IEnumerable<VehicleUtilizationDto>> GetVehicleUtilizationAsync(CancellationToken ct = default)
    {
        var vehicles = (await _repo.GetVehiclesWithCategoryAsync(ct)).ToList();
        var bookings = (await _repo.GetBookingsInRangeAsync(
            DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow, ct)).ToList();

        return vehicles.Select(v =>
        {
            var count = bookings.Count(b => b.VehicleId == v.Id);
            return new VehicleUtilizationDto
            {
                VehicleId = v.Id,
                RegistrationNumber = v.RegistrationNumber,
                VehicleName = v.Name,
                CategoryName = v.Category?.Name ?? string.Empty,
                TotalBookings = count,
                UtilizationRate = vehicles.Count == 0 ? 0 : Math.Round((decimal)count / 30 * 100, 2)
            };
        }).OrderByDescending(x => x.TotalBookings);
    }

    public async Task<CustomerReportDto> GetCustomerReportAsync(CancellationToken ct = default)
    {
        var customers = (await _repo.GetCustomersAsync(ct)).ToList();
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        return new CustomerReportDto
        {
            TotalCustomers = customers.Count,
            NewCustomersThisMonth = customers.Count(c => c.CreatedAt >= startOfMonth),
            ActiveCustomers = customers.Count(c => c.Status == CustomerStatus.Active),
            VerifiedCustomers = customers.Count(c => c.VerificationStatus == VerificationStatus.Verified),
            TopCustomers = new List<TopCustomerDto>()
        };
    }

    public async Task<PaymentReportDto> GetPaymentReportAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        from ??= DateTime.UtcNow.AddMonths(-1);
        to ??= DateTime.UtcNow;

        var payments = (await _repo.GetPaymentsInRangeAsync(from.Value, to.Value, ct)).ToList();

        return new PaymentReportDto
        {
            TotalPayments = payments.Sum(p => p.Amount),
            PendingPayments = payments.Where(p => p.Status == PaymentStatus.Pending).Sum(p => p.Amount),
            CompletedPayments = payments.Where(p => p.Status == PaymentStatus.Success).Sum(p => p.Amount),
            TotalTransactions = payments.Count,
            DataPoints = payments
                .GroupBy(p => p.CreatedAt.Date)
                .Select(g => new PaymentDataPointDto
                {
                    Date = g.Key,
                    Amount = g.Sum(x => x.Amount),
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList()
        };
    }

    public async Task<MaintenanceReportDto> GetMaintenanceReportAsync(CancellationToken ct = default)
    {
        // Placeholder until Anbu exposes a maintenance repository/query surface
        return await Task.FromResult(new MaintenanceReportDto());
    }

    public async Task<IEnumerable<BranchPerformanceDto>> GetBranchPerformanceAsync(CancellationToken ct = default)
    {
        var branches = (await _repo.GetBranchesAsync(ct)).ToList();
        var bookings = (await _repo.GetBookingsInRangeAsync(
            DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow, ct)).ToList();

        return branches.Select(b => new BranchPerformanceDto
        {
            BranchId = b.Id,
            BranchName = b.Name,
            City = b.City,
            TotalBookings = bookings.Count(x => x.PickupBranchId == b.Id),
            TotalRevenue = bookings
                .Where(x => x.PickupBranchId == b.Id && x.Status == BookingStatus.Completed)
                .Sum(x => x.TotalAmount),
            VehiclesAvailable = 0   // TODO: needs a repo method to count per-branch available vehicles
        });
    }
}
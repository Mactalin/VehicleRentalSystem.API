using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _db;
    public ReportRepository(AppDbContext db) => _db = db;

    // ---- Dashboard primitives ----
    public async Task<int> GetTotalVehiclesAsync(CancellationToken ct = default)
        => await _db.Vehicles.CountAsync(ct);

    public async Task<int> GetAvailableVehiclesAsync(CancellationToken ct = default)
        => await _db.Vehicles.CountAsync(v => v.Status == VehicleStatus.Available, ct);

    public async Task<int> GetActiveRentalsAsync(CancellationToken ct = default)
        => await _db.Bookings.CountAsync(b =>
            b.Status == BookingStatus.Confirmed ||
            b.Status == BookingStatus.PickedUp, ct);

    public async Task<int> GetTotalCustomersAsync(CancellationToken ct = default)
        => await _db.Customers.CountAsync(ct);

    public async Task<int> GetTodayBookingsAsync(CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _db.Bookings.CountAsync(b => b.StartAt.Date == today, ct);
    }

    public async Task<decimal> GetMonthlyRevenueAsync(CancellationToken ct = default)
    {
        var start = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        return await _db.Bookings
            .Where(b => b.StartAt >= start && b.Status == BookingStatus.Completed)
            .SumAsync(b => (decimal?)b.TotalAmount, ct) ?? 0m;
    }

    public async Task<decimal> GetPendingPaymentsAsync(CancellationToken ct = default)
        => await _db.Payments
            .Where(p => p.Status == PaymentStatus.Pending)
            .SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;

    public async Task<int> GetVehiclesUnderMaintenanceAsync(CancellationToken ct = default)
        => await _db.Vehicles.CountAsync(v => v.Status == VehicleStatus.Maintenance, ct);

    // ---- Raw data for Application-layer composition ----
    public async Task<IEnumerable<Booking>> GetBookingsInRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await _db.Bookings
            .Where(b => b.StartAt >= from && b.StartAt <= to)
            .ToListAsync(ct);

    public async Task<IEnumerable<Vehicle>> GetVehiclesWithCategoryAsync(CancellationToken ct = default)
        => await _db.Vehicles
            .Include(v => v.Category)
            .ToListAsync(ct);

    public async Task<IEnumerable<Customer>> GetCustomersAsync(CancellationToken ct = default)
        => await _db.Customers.ToListAsync(ct);

    public async Task<IEnumerable<Invoice>> GetInvoicesInRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await _db.Invoices
            .Include(i => i.Items)
            .Where(i => i.IssuedAt >= from && i.IssuedAt <= to)
            .ToListAsync(ct);

    public async Task<IEnumerable<Payment>> GetPaymentsInRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await _db.Payments
            .Where(p => p.CreatedAt >= from && p.CreatedAt <= to)
            .ToListAsync(ct);

    public async Task<IEnumerable<Branch>> GetBranchesAsync(CancellationToken ct = default)
        => await _db.Branches.ToListAsync(ct);
}
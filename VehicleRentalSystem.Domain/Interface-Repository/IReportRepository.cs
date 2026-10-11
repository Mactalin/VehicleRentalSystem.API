using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Interface_Repository
{
    public interface IReportRepository
    {
        // Dashboard summary (primitives only — no DTOs)
        Task<int> GetTotalVehiclesAsync(CancellationToken ct = default);
        Task<int> GetAvailableVehiclesAsync(CancellationToken ct = default);
        Task<int> GetActiveRentalsAsync(CancellationToken ct = default);
        Task<int> GetTotalCustomersAsync(CancellationToken ct = default);
        Task<int> GetTodayBookingsAsync(CancellationToken ct = default);
        Task<decimal> GetMonthlyRevenueAsync(CancellationToken ct = default);
        Task<decimal> GetPendingPaymentsAsync(CancellationToken ct = default);
        Task<int> GetVehiclesUnderMaintenanceAsync(CancellationToken ct = default);

        // Raw data for Application-layer composition
        Task<IEnumerable<Booking>> GetBookingsInRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
        Task<IEnumerable<Vehicle>> GetVehiclesWithCategoryAsync(CancellationToken ct = default);
        Task<IEnumerable<Customer>> GetCustomersAsync(CancellationToken ct = default);
        Task<IEnumerable<Invoice>> GetInvoicesInRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
        Task<IEnumerable<Payment>> GetPaymentsInRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
        Task<IEnumerable<Branch>> GetBranchesAsync(CancellationToken ct = default);
    }
}
using VehicleRentalSystem.Application.DTOs.Reports;

namespace VehicleRentalSystem.Application.Interface_Service;

public interface IReportService
{
    Task<RevenueReportDto> GetRevenueReportAsync(DateTime? from, DateTime? to, CancellationToken ct = default);
    Task<BookingReportDto> GetBookingReportAsync(DateTime? from, DateTime? to, CancellationToken ct = default);
    Task<IEnumerable<VehicleUtilizationDto>> GetVehicleUtilizationAsync(CancellationToken ct = default);
    Task<CustomerReportDto> GetCustomerReportAsync(CancellationToken ct = default);
    Task<PaymentReportDto> GetPaymentReportAsync(DateTime? from, DateTime? to, CancellationToken ct = default);
    Task<MaintenanceReportDto> GetMaintenanceReportAsync(CancellationToken ct = default);
    Task<IEnumerable<BranchPerformanceDto>> GetBranchPerformanceAsync(CancellationToken ct = default);
}
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.DTOs.Reports;

namespace VehicleRentalSystem.Application.ServiceInterfaces;

public interface IReportService
{
    Task<RevenueReportDto> GetRevenueReportAsync(DateTime? from = null, DateTime? to = null);
    Task<BookingReportDto> GetBookingReportAsync(DateTime? from = null, DateTime? to = null);
    Task<IEnumerable<VehicleUtilizationDto>> GetVehicleUtilizationAsync();
    Task<CustomerReportDto> GetCustomerReportAsync();
    Task<PaymentReportDto> GetPaymentReportAsync();
    Task<MaintenanceReportDto> GetMaintenanceReportAsync();
    Task<IEnumerable<BranchPerformanceDto>> GetBranchPerformanceAsync();

}
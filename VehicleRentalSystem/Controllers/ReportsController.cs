using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.ServiceInterfaces;

namespace VehicleRentalSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;

    public ReportsController(IReportService service) => _service = service;

    [HttpGet("revenue")]
    public async Task<IActionResult> Revenue([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _service.GetRevenueReportAsync(from, to));

    [HttpGet("bookings")]
    public async Task<IActionResult> Bookings([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _service.GetBookingReportAsync(from, to));

    [HttpGet("vehicle-utilization")]
    public async Task<IActionResult> VehicleUtilization()
        => Ok(await _service.GetVehicleUtilizationAsync());

    [HttpGet("customers")]
    public async Task<IActionResult> Customers()
        => Ok(await _service.GetCustomerReportAsync());

    [HttpGet("payments")]
    public async Task<IActionResult> Payments()
        => Ok(await _service.GetPaymentReportAsync());

    [HttpGet("maintenance")]
    public async Task<IActionResult> Maintenance()
        => Ok(await _service.GetMaintenanceReportAsync());

    [HttpGet("branch-performance")]
    public async Task<IActionResult> BranchPerformance()
        => Ok(await _service.GetBranchPerformanceAsync());
}
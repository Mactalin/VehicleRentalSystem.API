using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Authorization;

namespace VehicleRentalSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;
    private readonly CancellationToken _ct;

    public ReportsController(IReportService service, IHttpContextAccessor accessor)
    {
        _service = service;
        _ct = accessor.HttpContext?.RequestAborted ?? CancellationToken.None;
    }

    [HttpGet("revenue")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Revenue([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _service.GetRevenueReportAsync(from, to, _ct));

    [HttpGet("bookings")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Bookings([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _service.GetBookingReportAsync(from, to, _ct));

    [HttpGet("vehicle-utilization")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> VehicleUtilization()
        => Ok(await _service.GetVehicleUtilizationAsync(_ct));

    [HttpGet("customers")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Customers()
        => Ok(await _service.GetCustomerReportAsync(_ct));

    [HttpGet("payments")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Payments([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _service.GetPaymentReportAsync(from, to, _ct));

    [HttpGet("maintenance")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Maintenance()
        => Ok(await _service.GetMaintenanceReportAsync(_ct));

    [HttpGet("branch-performance")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> BranchPerformance()
        => Ok(await _service.GetBranchPerformanceAsync(_ct));
}
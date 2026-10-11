using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Authorization;

namespace VehicleRentalSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;
    private readonly CancellationToken _ct;

    public DashboardController(IDashboardService service, IHttpContextAccessor accessor)
    {
        _service = service;
        _ct = accessor.HttpContext?.RequestAborted ?? CancellationToken.None;
    }

    [HttpGet("stats")]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<IActionResult> GetStats() => Ok(await _service.GetStatsAsync(_ct));
}
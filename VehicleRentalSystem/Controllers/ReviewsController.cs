using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.DTOs.Reviews;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Authorization;

namespace VehicleRentalSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _service;
    private readonly CancellationToken _ct;

    public ReviewsController(IReviewService service, IHttpContextAccessor accessor)
    {
        _service = service;
        _ct = accessor.HttpContext?.RequestAborted ?? CancellationToken.None;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync(_ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id, _ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>All reviews for a specific vehicle (public).</summary>
    [HttpGet("vehicle/{vehicleId:int}")]
    public async Task<IActionResult> GetByVehicle(int vehicleId)
        => Ok(await _service.GetByVehicleIdAsync(vehicleId, _ct));

    /// <summary>Rating summary for a vehicle (public).</summary>
    [HttpGet("vehicle/{vehicleId:int}/summary")]
    public async Task<IActionResult> GetVehicleSummary(int vehicleId)
        => Ok(await _service.GetVehicleSummaryAsync(vehicleId, _ct));

    /// <summary>All reviews for a specific driver (public).</summary>
    [HttpGet("driver/{driverId:int}")]
    public async Task<IActionResult> GetByDriver(int driverId)
        => Ok(await _service.GetByDriverIdAsync(driverId, _ct));

    /// <summary>Rating summary for a driver (public).</summary>
    [HttpGet("driver/{driverId:int}/summary")]
    public async Task<IActionResult> GetDriverSummary(int driverId)
        => Ok(await _service.GetDriverSummaryAsync(driverId, _ct));

    /// <summary>All reviews written by a customer.</summary>
    [HttpGet("customer/{customerId:int}")]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<IActionResult> GetByCustomer(int customerId)
        => Ok(await _service.GetByCustomerIdAsync(customerId, _ct));

    /// <summary>Submit a review (customer action).</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
    {
        var created = await _service.CreateAsync(dto, _ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id, _ct);
        return NoContent();
    }
}
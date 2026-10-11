using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.DTOs.Drivers;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Authorization;

namespace VehicleRentalSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriversController : ControllerBase
{
    private readonly IDriverService _service;
    private readonly CancellationToken _ct;

    public DriversController(IDriverService service, IHttpContextAccessor accessor)
    {
        _service = service;
        _ct = accessor.HttpContext?.RequestAborted ?? CancellationToken.None;
    }

    /// <summary>Get all drivers (admin/staff only).</summary>
    [HttpGet]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync(_ct));

    /// <summary>Get available drivers for assignment.</summary>
    [HttpGet("available")]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<IActionResult> GetAvailable() => Ok(await _service.GetAvailableAsync(_ct));

    /// <summary>Get a single driver.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id, _ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>Create a new driver.</summary>
    [HttpPost]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Create([FromBody] CreateDriverDto dto)
    {
        var created = await _service.CreateAsync(dto, _ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Update an existing driver.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDriverDto dto)
        => Ok(await _service.UpdateAsync(id, dto, _ct));

    /// <summary>Soft-delete a driver.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id, _ct);
        return NoContent();
    }
}
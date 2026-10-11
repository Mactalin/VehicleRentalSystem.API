using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.DTOs.Complaints;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Authorization;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComplaintsController : ControllerBase
{
    private readonly IComplaintService _service;
    private readonly CancellationToken _ct;

    public ComplaintsController(IComplaintService service, IHttpContextAccessor accessor)
    {
        _service = service;
        _ct = accessor.HttpContext?.RequestAborted ?? CancellationToken.None;
    }

    [HttpGet]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync(_ct));

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id, _ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("status/{status}")]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<IActionResult> GetByStatus(ComplaintStatus status)
        => Ok(await _service.GetByStatusAsync(status, _ct));

    [HttpGet("customer/{customerId:int}")]
    [Authorize]
    public async Task<IActionResult> GetByCustomer(int customerId)
        => Ok(await _service.GetByCustomerIdAsync(customerId, _ct));

    /// <summary>Customer files a complaint.</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateComplaintDto dto)
    {
        var created = await _service.CreateAsync(dto, _ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Admin posts an official response.</summary>
    [HttpPut("{id:int}/respond")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Respond(int id, [FromBody] RespondToComplaintDto dto)
        => Ok(await _service.RespondAsync(id, dto, _ct));

    [HttpPut("{id:int}/resolve")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Resolve(int id)
        => Ok(await _service.ResolveAsync(id, _ct));

    [HttpPut("{id:int}/close")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Close(int id)
        => Ok(await _service.CloseAsync(id, _ct));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id, _ct);
        return NoContent();
    }
}
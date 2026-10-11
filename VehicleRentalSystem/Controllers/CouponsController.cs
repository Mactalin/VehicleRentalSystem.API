using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.DTOs.Coupons;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Authorization;

namespace VehicleRentalSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CouponsController : ControllerBase
{
    private readonly ICouponService _service;
    private readonly CancellationToken _ct;

    public CouponsController(ICouponService service, IHttpContextAccessor accessor)
    {
        _service = service;
        _ct = accessor.HttpContext?.RequestAborted ?? CancellationToken.None;
    }

    [HttpGet]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync(_ct));

    [HttpGet("active")]
    public async Task<IActionResult> GetActive() => Ok(await _service.GetActiveAsync(_ct));

    [HttpGet("{id:int}")]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id, _ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("code/{code}")]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<IActionResult> GetByCode(string code)
    {
        var dto = await _service.GetByCodeAsync(code, _ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>Create a new coupon. Admin/Manager only.</summary>
    [HttpPost]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Create([FromBody] CreateCouponDto dto)
    {
        var created = await _service.CreateAsync(dto, _ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCouponDto dto)
        => Ok(await _service.UpdateAsync(id, dto, _ct));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id, _ct);
        return NoContent();
    }

    /// <summary>Validate a coupon at checkout. Anyone authenticated may call.</summary>
    [HttpPost("validate")]
    [Authorize]
    public async Task<IActionResult> Validate([FromBody] ValidateCouponDto dto)
        => Ok(await _service.ValidateAsync(dto, _ct));
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.DTOs.Offers;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Authorization;

namespace VehicleRentalSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OffersController : ControllerBase
{
    private readonly IOfferService _service;
    private readonly CancellationToken _ct;

    public OffersController(IOfferService service, IHttpContextAccessor accessor)
    {
        _service = service;
        _ct = accessor.HttpContext?.RequestAborted ?? CancellationToken.None;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync(_ct));

    [HttpGet("running")]
    public async Task<IActionResult> GetRunning() => Ok(await _service.GetRunningAsync(_ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id, _ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Create([FromBody] CreateOfferDto dto)
    {
        var created = await _service.CreateAsync(dto, _ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOfferDto dto)
        => Ok(await _service.UpdateAsync(id, dto, _ct));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id, _ct);
        return NoContent();
    }
}
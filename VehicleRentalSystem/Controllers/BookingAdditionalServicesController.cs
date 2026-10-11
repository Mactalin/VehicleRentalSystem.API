using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.DTOs.BookingServices;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Authorization;

namespace VehicleRentalSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingAdditionalServicesController : ControllerBase
{
    private readonly IBookingAdditionalServiceService _service;
    private readonly CancellationToken _ct;

    public BookingAdditionalServicesController(IBookingAdditionalServiceService service, IHttpContextAccessor accessor)
    {
        _service = service;
        _ct = accessor.HttpContext?.RequestAborted ?? CancellationToken.None;
    }

    [HttpGet("booking/{bookingId:int}")]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<IActionResult> GetByBooking(int bookingId)
        => Ok(await _service.GetByBookingIdAsync(bookingId, _ct));

    [HttpPost]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<IActionResult> Add([FromBody] AddServiceToBookingDto dto)
        => Ok(await _service.AddAsync(dto, _ct));

    [HttpDelete("{bookingId:int}/{additionalServiceId:int}")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Remove(int bookingId, int additionalServiceId)
    {
        await _service.RemoveAsync(bookingId, additionalServiceId, _ct);
        return NoContent();
    }
}
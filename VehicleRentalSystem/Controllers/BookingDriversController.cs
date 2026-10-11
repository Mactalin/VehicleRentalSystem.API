using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.DTOs.BookingDrivers;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Authorization;

namespace VehicleRentalSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingDriversController : ControllerBase
{
    private readonly IBookingDriverService _service;
    private readonly CancellationToken _ct;

    public BookingDriversController(IBookingDriverService service, IHttpContextAccessor accessor)
    {
        _service = service;
        _ct = accessor.HttpContext?.RequestAborted ?? CancellationToken.None;
    }

    /// <summary>Get drivers assigned to a booking.</summary>
    [HttpGet("booking/{bookingId:int}")]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<IActionResult> GetByBooking(int bookingId)
        => Ok(await _service.GetByBookingIdAsync(bookingId, _ct));

    /// <summary>Get all bookings a driver has been assigned to.</summary>
    [HttpGet("driver/{driverId:int}")]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<IActionResult> GetByDriver(int driverId)
        => Ok(await _service.GetByDriverIdAsync(driverId, _ct));

    /// <summary>Assign a driver to a booking.</summary>
    [HttpPost("assign")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Assign([FromBody] AssignDriverDto dto)
        => Ok(await _service.AssignAsync(dto, _ct));

    /// <summary>Unassign a driver from a booking.</summary>
    [HttpDelete("unassign/{bookingId:int}/{driverId:int}")]
    [Authorize(Policy = Policies.AdminOrManager)]
    public async Task<IActionResult> Unassign(int bookingId, int driverId)
    {
        await _service.UnassignAsync(bookingId, driverId, _ct);
        return NoContent();
    }
}
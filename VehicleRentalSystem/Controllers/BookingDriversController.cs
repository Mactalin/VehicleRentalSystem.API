using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.DTOs;
using VehicleRentalSystem.Application.ServiceInterfaces;

namespace VehicleRentalSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingDriversController : ControllerBase
{
    private readonly IBookingDriverService _service;

    public BookingDriversController(IBookingDriverService service) => _service = service;

    [HttpGet("booking/{bookingId}")]
    public async Task<IActionResult> GetByBooking(int bookingId)
        => Ok(await _service.GetByBookingIdAsync(bookingId));

    [HttpPost("assign")]
    public async Task<IActionResult> Assign([FromBody] AssignDriverDto dto)
    {
        try { return Ok(await _service.AssignDriverAsync(dto)); }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }

    [HttpDelete("unassign/{bookingId}/{driverId}")]
    public async Task<IActionResult> Unassign(int bookingId, int driverId)
    {
        try { await _service.UnassignDriverAsync(bookingId, driverId); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }
}
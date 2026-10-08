using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.DTOs;
using VehicleRentalSystem.Application.ServiceInterfaces;

namespace VehicleRentalSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingAdditionalServicesController : ControllerBase
{
    private readonly IBookingAdditionalServiceService _service;

    public BookingAdditionalServicesController(IBookingAdditionalServiceService service) => _service = service;

    [HttpGet("booking/{bookingId}")]
    public async Task<IActionResult> GetByBooking(int bookingId)
        => Ok(await _service.GetByBookingIdAsync(bookingId));

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddServiceToBookingDto dto)
    {
        try { return Ok(await _service.AddServiceAsync(dto)); }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }

    [HttpDelete("{bookingId}/{additionalServiceId}")]
    public async Task<IActionResult> Remove(int bookingId, int additionalServiceId)
    {
        try { await _service.RemoveServiceAsync(bookingId, additionalServiceId); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }
}
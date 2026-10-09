using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.Dtos.Bookings;
using VehicleRentalSystem.Application.Dtos.Payments;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Auth;
using VehicleRentalSystem.Authorization;

namespace VehicleRentalSystem.Controllers;

[Route("api/bookings")]
public class BookingsController : ApiControllerBase
{
    private readonly IBookingService _bookings;
    private readonly IInvoiceService _invoices;

    public BookingsController(IBookingService bookings, IInvoiceService invoices)
    {
        _bookings = bookings;
        _invoices = invoices;
    }

    // ---------- Customer flow ----------

    [HttpPost("quote")]
    public async Task<ActionResult<PriceQuoteDto>> Quote(PriceQuoteRequestDto dto, CancellationToken ct) =>
        Ok(await _bookings.QuoteAsync(dto, ct));

    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create(CreateBookingDto dto, CancellationToken ct)
    {
        if (!IsStaff)
        {
            dto.CustomerId = await GetMyCustomerIdAsync(ct);    // customers can only book for themselves
        }
        else if (dto.CustomerId <= 0)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Customer required",
                           detail: "Staff must provide a customerId when booking on behalf of a customer.");
        }

        var booking = await _bookings.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
    }

    [HttpGet("my")]
    public async Task<ActionResult<IReadOnlyList<BookingSummaryDto>>> GetMine(CancellationToken ct) =>
        Ok(await _bookings.GetByCustomerAsync(await GetMyCustomerIdAsync(ct), ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDto>> GetById(int id, CancellationToken ct)
    {
        var booking = await LoadAuthorizedAsync(id, ct);
        return booking is null ? ForbiddenResult() : Ok(booking);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<BookingDto>> Modify(int id, ModifyBookingDto dto, CancellationToken ct)
    {
        if (await LoadAuthorizedAsync(id, ct) is null) return ForbiddenResult();
        return Ok(await _bookings.ModifyAsync(id, dto, CurrentUserId, ct));
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<CancellationResultDto>> Cancel(int id, CancelBookingDto dto, CancellationToken ct)
    {
        if (await LoadAuthorizedAsync(id, ct) is null) return ForbiddenResult();
        return Ok(await _bookings.CancelAsync(id, dto, ct));
    }

    [HttpGet("{id:int}/invoice")]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(int id, CancellationToken ct)
    {
        if (await LoadAuthorizedAsync(id, ct) is null) return ForbiddenResult();
        return Ok(await _invoices.GetByBookingAsync(id, ct));
    }

    // ---------- Counter staff flow ----------

    [HttpPost("{id:int}/pickup")]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<ActionResult<BookingDto>> Pickup(int id, PickupDto dto, CancellationToken ct) =>
        Ok(await _bookings.PickupAsync(id, dto, CurrentUserId, ct));

    [HttpPost("{id:int}/return")]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<ActionResult<ReturnSummaryDto>> Return(int id, ReturnDto dto, CancellationToken ct) =>
        Ok(await _bookings.ReturnAsync(id, dto, CurrentUserId, ct));

    [HttpPost("{id:int}/complete")]
    [Authorize(Policy = Policies.StaffAndAbove)]
    public async Task<ActionResult<BookingDto>> Complete(int id, CancellationToken ct) =>
        Ok(await _bookings.CompleteAsync(id, ct));

    // ---------- Helper ----------

    /// Returns the booking, or null when it belongs to someone else and the caller is not staff.
    private async Task<BookingDto?> LoadAuthorizedAsync(int id, CancellationToken ct)
    {
        var booking = await _bookings.GetByIdAsync(id, ct);
        return await CanAccessCustomerAsync(booking.CustomerId, ct) ? booking : null;
    }
}
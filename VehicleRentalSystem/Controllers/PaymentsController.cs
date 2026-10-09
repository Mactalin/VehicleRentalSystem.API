using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.Dtos.Payments;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Authorization;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Controllers
{
    [Route("api/payments")]
    public class PaymentsController : ApiControllerBase
    {
        private readonly IPaymentService _payments;
        private readonly IBookingService _bookings;

        public PaymentsController(IPaymentService payments, IBookingService bookings)
        {
            _payments = payments;
            _bookings = bookings;
        }

        /// The amount is decided by the server from the booking, never by the client.
        [HttpPost]
        public async Task<ActionResult<PaymentDto>> Create(CreatePaymentDto dto, CancellationToken ct)
        {
            if (!await CanAccessBookingAsync(dto.BookingId, ct)) return ForbiddenResult();

            // Cash is taken at the counter, so only staff can record it
            if (dto.Method == PaymentMethod.Cash && !IsStaff) return ForbiddenResult();

            var payment = await _payments.CreatePaymentAsync(dto, ct);

            return payment.Status == PaymentStatus.Success
                ? StatusCode(StatusCodes.Status201Created, payment)
                : StatusCode(StatusCodes.Status402PaymentRequired, payment);   // gateway declined, details in the body
        }

        [HttpPost("refund")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<ActionResult<RefundDto>> Refund(RefundRequestDto dto, CancellationToken ct) =>
            Ok(await _payments.RefundAsync(dto, ct));

        [HttpGet("booking/{bookingId:int}")]
        public async Task<ActionResult<IReadOnlyList<PaymentDto>>> GetByBooking(int bookingId, CancellationToken ct)
        {
            if (!await CanAccessBookingAsync(bookingId, ct)) return ForbiddenResult();
            return Ok(await _payments.GetByBookingAsync(bookingId, ct));
        }

        /// What is paid, what is still owed, and how much deposit comes back
        [HttpGet("booking/{bookingId:int}/settlement")]
        public async Task<ActionResult<SettlementDto>> GetSettlement(int bookingId, CancellationToken ct)
        {
            if (!await CanAccessBookingAsync(bookingId, ct)) return ForbiddenResult();
            return Ok(await _payments.GetSettlementAsync(bookingId, ct));
        }

        private async Task<bool> CanAccessBookingAsync(int bookingId, CancellationToken ct)
        {
            var booking = await _bookings.GetByIdAsync(bookingId, ct);
            return await CanAccessCustomerAsync(booking.CustomerId, ct);
        }
    }
}


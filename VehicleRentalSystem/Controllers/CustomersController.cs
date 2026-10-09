using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.Dtos.Bookings;
using VehicleRentalSystem.Application.Dtos.Customers;
using VehicleRentalSystem.Application.Dtos.Payments;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Authorization;

namespace VehicleRentalSystem.Controllers
{
    [Route("api/customers")]
    public class CustomersController : ApiControllerBase
    {
        private readonly ICustomerService _customers;
        private readonly IPaymentService _payments;

        public CustomersController(ICustomerService customers, IPaymentService payments)
        {
            _customers = customers;
            _payments = payments;
        }

        // ---------- Profile ----------

        [HttpPost]
        public async Task<ActionResult<CustomerDto>> Register(CreateCustomerDto dto, CancellationToken ct)
        {
            dto.UserId = CurrentUserId;     // never trust a user id sent in the body
            var created = await _customers.RegisterAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("me")]
        public async Task<ActionResult<CustomerDto>> GetMine(CancellationToken ct) =>
            Ok(await _customers.GetByUserIdAsync(CurrentUserId, ct));

        [HttpGet]
        [Authorize(Policy = Policies.StaffAndAbove)]
        public async Task<ActionResult<PagedResult<CustomerSummaryDto>>> GetPaged(
            [FromQuery] CustomerFilterDto filter, CancellationToken ct) =>
            Ok(await _customers.GetPagedAsync(filter, ct));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerDto>> GetById(int id, CancellationToken ct)
        {
            if (!await CanAccessCustomerAsync(id, ct)) return ForbiddenResult();
            return Ok(await _customers.GetByIdAsync(id, ct));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CustomerDto>> Update(int id, UpdateCustomerDto dto, CancellationToken ct)
        {
            if (!await CanAccessCustomerAsync(id, ct)) return ForbiddenResult();
            return Ok(await _customers.UpdateAsync(id, dto, ct));
        }

        /// Suspend or block an account
        [HttpPatch("{id:int}/status")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<ActionResult<CustomerDto>> SetStatus(int id, UpdateCustomerStatusDto dto, CancellationToken ct) =>
            Ok(await _customers.SetStatusAsync(id, dto, ct));

        // ---------- KYC and licence ----------

        [HttpPost("{id:int}/kyc")]
        public async Task<ActionResult<KycDocumentDto>> AddKyc(int id, KycUploadDto dto, CancellationToken ct)
        {
            if (!await CanAccessCustomerAsync(id, ct)) return ForbiddenResult();
            var doc = await _customers.AddKycDocumentAsync(id, dto, ct);
            return StatusCode(StatusCodes.Status201Created, doc);
        }

        [HttpPost("{id:int}/licences")]
        public async Task<ActionResult<DrivingLicenceDto>> AddLicence(int id, LicenceUploadDto dto, CancellationToken ct)
        {
            if (!await CanAccessCustomerAsync(id, ct)) return ForbiddenResult();
            var licence = await _customers.AddLicenceAsync(id, dto, ct);
            return StatusCode(StatusCodes.Status201Created, licence);
        }

        [HttpPatch("{id:int}/kyc/{documentId:int}/review")]
        [Authorize(Policy = Policies.StaffAndAbove)]
        public async Task<ActionResult<CustomerDto>> ReviewKyc(
            int id, int documentId, ReviewDocumentDto dto, CancellationToken ct) =>
            Ok(await _customers.ReviewKycAsync(id, documentId, dto, CurrentUserId, ct));

        [HttpPatch("{id:int}/licences/{licenceId:int}/review")]
        [Authorize(Policy = Policies.StaffAndAbove)]
        public async Task<ActionResult<CustomerDto>> ReviewLicence(
            int id, int licenceId, ReviewDocumentDto dto, CancellationToken ct) =>
            Ok(await _customers.ReviewLicenceAsync(id, licenceId, dto, CurrentUserId, ct));

        // ---------- History ----------

        [HttpGet("{id:int}/bookings")]
        public async Task<ActionResult<IReadOnlyList<BookingSummaryDto>>> GetRentalHistory(int id, CancellationToken ct)
        {
            if (!await CanAccessCustomerAsync(id, ct)) return ForbiddenResult();
            return Ok(await _customers.GetRentalHistoryAsync(id, ct));
        }

        [HttpGet("{id:int}/payments")]
        public async Task<ActionResult<IReadOnlyList<PaymentDto>>> GetPaymentHistory(int id, CancellationToken ct)
        {
            if (!await CanAccessCustomerAsync(id, ct)) return ForbiddenResult();
            return Ok(await _payments.GetByCustomerAsync(id, ct));
        }
    }
}

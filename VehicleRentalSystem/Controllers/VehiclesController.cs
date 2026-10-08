using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.Dtos.Vehicle;
using VehicleRentalSystem.Application.Exceptions;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Authorization;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicles;

        public VehiclesController(IVehicleService vehicles) => _vehicles = vehicles;

        // ----- Public catalog -----

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAll(
            [FromQuery] VehicleStatus? status,
            [FromQuery] int? branchId,
            [FromQuery] int? categoryId,
            [FromQuery] bool includeInactive = false)
        {
            // Only staff can see deactivated vehicles
            var canSeeInactive = includeInactive && User.IsStaffOrAbove();
            return Ok(await _vehicles.GetAllAsync(status, branchId, categoryId, canSeeInactive));
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<VehicleDto>> GetById(int id) =>
            Ok(await _vehicles.GetByIdAsync(id));

        // ----- Admin / Manager: manage vehicles -----

        [HttpPost]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<ActionResult<VehicleDto>> Create(SaveVehicleDto dto)
        {
            var created = await _vehicles.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<ActionResult<VehicleDto>> Update(int id, SaveVehicleDto dto) =>
            Ok(await _vehicles.UpdateAsync(id, dto));

        [HttpDelete("{id:int}")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<IActionResult> Delete(int id)
        {
            await _vehicles.DeleteAsync(id);
            return NoContent();
        }

        // ----- Staff and above: status -----

        [HttpPatch("{id:int}/status")]
        [Authorize(Policy = Policies.StaffAndAbove)]
        public async Task<IActionResult> UpdateStatus(int id, UpdateVehicleStatusDto dto)
        {
            await _vehicles.UpdateStatusAsync(id, dto.Status);
            return NoContent();
        }

        // ----- Images (Admin / Manager) -----

        [HttpPost("{id:int}/images")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<ActionResult<VehicleImageDto>> AddImage(int id, AddVehicleImageDto dto) =>
            Ok(await _vehicles.AddImageAsync(id, dto));

        [HttpPatch("{id:int}/images/{imageId:int}/primary")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<IActionResult> SetPrimaryImage(int id, int imageId)
        {
            await _vehicles.SetPrimaryImageAsync(id, imageId);
            return NoContent();
        }

        [HttpDelete("{id:int}/images/{imageId:int}")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<IActionResult> DeleteImage(int id, int imageId)
        {
            await _vehicles.DeleteImageAsync(id, imageId);
            return NoContent();
        }

        // ----- Documents -----

        // Insurance / RC / PUC expiring soon (includes already expired)
        [HttpGet("documents/expiring")]
        [Authorize(Policy = Policies.StaffAndAbove)]
        public async Task<ActionResult<IEnumerable<ExpiringDocumentDto>>> GetExpiringDocuments(
            [FromQuery] int withinDays = 30) =>
            Ok(await _vehicles.GetExpiringDocumentsAsync(withinDays));

        [HttpPost("{id:int}/documents")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<ActionResult<VehicleDocumentDto>> AddDocument(int id, SaveVehicleDocumentDto dto) =>
            Ok(await _vehicles.AddDocumentAsync(id, dto));

        [HttpPut("{id:int}/documents/{documentId:int}")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<ActionResult<VehicleDocumentDto>> UpdateDocument(
            int id, int documentId, SaveVehicleDocumentDto dto) =>
            Ok(await _vehicles.UpdateDocumentAsync(id, documentId, dto));

        [HttpDelete("{id:int}/documents/{documentId:int}")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<IActionResult> DeleteDocument(int id, int documentId)
        {
            await _vehicles.DeleteDocumentAsync(id, documentId);
            return NoContent();
        }
    }
}

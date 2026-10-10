using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.Dtos.Maintenance;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Authorization;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceService _maintenance;

        public MaintenanceController(IMaintenanceService maintenance) => _maintenance = maintenance;

        // ----- Queries (Staff and above) -----

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MaintenanceDto>> GetById(int id) =>
            Ok(await _maintenance.GetByIdAsync(id));

        [HttpGet("vehicle/{vehicleId:int}")]
        public async Task<ActionResult<IEnumerable<MaintenanceDto>>> GetByVehicle(int vehicleId) =>
            Ok(await _maintenance.GetByVehicleAsync(vehicleId));

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<MaintenanceDto>>> GetByStatus(MaintenanceStatus status) =>
            Ok(await _maintenance.GetByStatusAsync(status));

        [HttpGet("upcoming")]
        public async Task<ActionResult<IEnumerable<MaintenanceDto>>> GetUpcoming([FromQuery] int withinDays = 30) =>
            Ok(await _maintenance.GetUpcomingAsync(withinDays));

        [HttpGet("vehicle/{vehicleId:int}/total-cost")]
        public async Task<ActionResult<decimal>> GetTotalCost(int vehicleId) =>
            Ok(await _maintenance.GetTotalCostAsync(vehicleId));

        // ----- Commands (Staff and above) -----

        [HttpPost]
        public async Task<ActionResult<MaintenanceDto>> Create(SaveMaintenanceDto dto)
        {
            var created = await _maintenance.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<MaintenanceDto>> Update(int id, SaveMaintenanceDto dto) =>
            Ok(await _maintenance.UpdateAsync(id, dto));

        // Start (InProgress) or Cancel
        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<MaintenanceDto>> UpdateStatus(int id, UpdateMaintenanceStatusDto dto) =>
            Ok(await _maintenance.UpdateStatusAsync(id, dto.Status));

        // Finish the job and record the final cost
        [HttpPatch("{id:int}/complete")]
        public async Task<ActionResult<MaintenanceDto>> Complete(int id, CompleteMaintenanceDto dto) =>
            Ok(await _maintenance.CompleteAsync(id, dto));

        // ----- Delete (Admin / Manager only) -----

        [HttpDelete("{id:int}")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<IActionResult> Delete(int id)
        {
            await _maintenance.DeleteAsync(id);
            return NoContent();
        }
    }
}

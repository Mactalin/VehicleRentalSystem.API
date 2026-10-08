using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.Dtos.Branch;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Authorization;

namespace VehicleRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchService _branches;

        public BranchesController(IBranchService branches) => _branches = branches;

        // Public: customers choose pickup / drop-off branches
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BranchDto>>> GetAll([FromQuery] string? city) =>
            Ok(await _branches.GetAllAsync(city));

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<BranchDto>> GetById(int id) =>
            Ok(await _branches.GetByIdAsync(id));

        [HttpPost]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<ActionResult<BranchDto>> Create(SaveBranchDto dto)
        {
            var created = await _branches.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<ActionResult<BranchDto>> Update(int id, SaveBranchDto dto) =>
            Ok(await _branches.UpdateAsync(id, dto));

        [HttpDelete("{id:int}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Delete(int id)
        {
            await _branches.DeleteAsync(id);
            return NoContent();
        }

        // ----- Operating hours -----

        [HttpGet("{id:int}/operating-hours")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<OperatingHourDto>>> GetOperatingHours(int id) =>
            Ok(await _branches.GetOperatingHoursAsync(id));

        [HttpPut("{id:int}/operating-hours")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<ActionResult<IEnumerable<OperatingHourDto>>> SetOperatingHours(
            int id, List<OperatingHourDto> hours) =>
            Ok(await _branches.SetOperatingHoursAsync(id, hours));
    }
}

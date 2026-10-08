using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.Dtos.User;
using VehicleRentalSystem.Application.Exceptions;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Authorization;

namespace VehicleRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _users;

        public UsersController(IUserService users) => _users = users;

        // ----- Any logged-in user: own profile -----

        [HttpGet("profile")]
        public async Task<ActionResult<UserDto>> GetProfile() =>
            Ok(await _users.GetByIdAsync(User.GetUserId()));

        [HttpPut("profile")]
        public async Task<ActionResult<UserDto>> UpdateProfile(UpdateProfileDto dto) =>
            Ok(await _users.UpdateProfileAsync(User.GetUserId(), dto));

        // ----- Admin / Manager: view users -----

        [HttpGet]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll() =>
            Ok(await _users.GetAllAsync());

        [HttpGet("{id:int}")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<ActionResult<UserDto>> GetById(int id) =>
            Ok(await _users.GetByIdAsync(id));

        // ----- Admin only: manage accounts -----

        [HttpGet("roles")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<ActionResult<IEnumerable<string>>> GetRoles() =>
            Ok(await _users.GetRolesAsync());

        [HttpPost]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<ActionResult<UserDto>> Create(CreateUserDto dto)
        {
            var created = await _users.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}/role")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<ActionResult<UserDto>> UpdateRole(int id, UpdateUserRoleDto dto) =>
            Ok(await _users.UpdateRoleAsync(id, dto, User.GetUserId()));

        [HttpPatch("{id:int}/activate")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Activate(int id)
        {
            await _users.SetActiveAsync(id, true, User.GetUserId());
            return NoContent();
        }

        [HttpPatch("{id:int}/deactivate")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _users.SetActiveAsync(id, false, User.GetUserId());
            return NoContent();
        }
    }
}

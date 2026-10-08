using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.Dtos.Auth;
using VehicleRentalSystem.Application.Dtos.User;
using VehicleRentalSystem.Application.Exceptions;
using VehicleRentalSystem.Application.Interface_Service;

namespace VehicleRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IUserService _users;

        public AuthController(IAuthService auth, IUserService users)
        {
            _auth = auth;
            _users = users;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto) =>
            Ok(await _auth.RegisterAsync(dto));

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto) =>
            Ok(await _auth.LoginAsync(dto));

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            await _auth.ChangePasswordAsync(User.GetUserId(), dto);
            return NoContent();
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> Me() =>
            Ok(await _users.GetByIdAsync(User.GetUserId()));
    }
}

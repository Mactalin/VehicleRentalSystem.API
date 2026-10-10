using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.User;

namespace VehicleRentalSystem.Application.Dtos.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = null!;
    }
}

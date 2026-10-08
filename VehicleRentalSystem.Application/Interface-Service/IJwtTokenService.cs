using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Application.Interface_Service
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}

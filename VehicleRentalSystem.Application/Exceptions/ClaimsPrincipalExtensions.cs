using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace VehicleRentalSystem.Application.Exceptions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id)
                ? id
                : throw new UnauthorizedException("Invalid token.");
        }

        public static bool IsStaffOrAbove(this ClaimsPrincipal user) =>
            user.IsInRole("Admin") || user.IsInRole("Manager") || user.IsInRole("Staff");
    }
}

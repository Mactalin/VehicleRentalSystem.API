using System.Security.Claims;

namespace VehicleRentalSystem.Auth
{
    public static class ClaimsPrincipalExtensions
    {
        private static readonly string[] UserIdClaimTypes =
            { ClaimTypes.NameIdentifier, "sub", "userId", "uid" };

        public static int? GetUserId(this ClaimsPrincipal user)
        {
            foreach (var type in UserIdClaimTypes)
            {
                if (int.TryParse(user.FindFirstValue(type), out var id))
                    return id;
            }
            return null;
        }

        public static IEnumerable<string> GetRoles(this ClaimsPrincipal user) =>
            user.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role" || c.Type == "roles")
                .Select(c => c.Value);

        public static bool HasRole(this ClaimsPrincipal user, string role) =>
            user.GetRoles().Contains(role, StringComparer.OrdinalIgnoreCase);

        public static bool IsAdmin(this ClaimsPrincipal user) => user.HasRole(Roles.Admin);

        public static bool IsStaff(this ClaimsPrincipal user) =>
            user.HasRole(Roles.Admin) || user.HasRole(Roles.Staff);
    }
}

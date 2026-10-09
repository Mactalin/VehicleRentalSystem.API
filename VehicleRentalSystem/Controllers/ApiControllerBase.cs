using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Auth;

namespace VehicleRentalSystem.Controllers
{
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected int CurrentUserId =>
            User.GetUserId() ?? throw new UnauthorizedAccessException("The token does not contain a user id.");

        protected bool IsStaff => User.IsStaff();

        /// The Customer.Id of the logged-in user. Throws NotFound (404) if they have no profile yet.
        protected async Task<int> GetMyCustomerIdAsync(CancellationToken ct)
        {
            var customers = HttpContext.RequestServices.GetRequiredService<ICustomerService>();
            var me = await customers.GetByUserIdAsync(CurrentUserId, ct);
            return me.Id;
        }

        /// Staff can access everything. A customer can only access their own records.
        protected async Task<bool> CanAccessCustomerAsync(int customerId, CancellationToken ct) =>
            IsStaff || await GetMyCustomerIdAsync(ct) == customerId;

        protected ActionResult ForbiddenResult() =>
            Problem(statusCode: StatusCodes.Status403Forbidden, title: "Forbidden",
                    detail: "You do not have access to this resource.");
    }
}

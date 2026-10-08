using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.Exceptions;

namespace VehicleRentalSystem.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var (status, title) = ex switch
                {
                    NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
                    BadRequestException => (StatusCodes.Status400BadRequest, "Bad Request"),
                    ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
                    UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                    _ => (StatusCodes.Status500InternalServerError, "Server Error")
                };

                if (status == StatusCodes.Status500InternalServerError)
                    _logger.LogError(ex, "Unhandled exception");

                if (context.Response.HasStarted) throw;

                context.Response.StatusCode = status;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = status,
                    Title = title,
                    // Never leak internal details for 500s
                    Detail = status == 500 ? "An unexpected error occurred." : ex.Message
                });
            }
        }
    }
}

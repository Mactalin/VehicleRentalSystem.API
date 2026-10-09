using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Exceptions;

namespace VehicleRentalSystem.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(
            RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                // Client disconnected. Nothing to send back.
            }
            catch (Exception ex)
            {
                await HandleAsync(context, ex);
            }
        }

        private async Task HandleAsync(HttpContext context, Exception ex)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogError(ex, "Exception after the response had started.");
                return;
            }

            var sql = ex.GetBaseException() as SqlException;

            var (status, title) = ex switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Not found"),
                BookingConflictException => (StatusCodes.Status409Conflict, "Booking conflict"),
                DomainException => (StatusCodes.Status400BadRequest, "Request not allowed"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "Concurrency conflict"),
                _ when sql is { Number: 1205 } => (StatusCodes.Status409Conflict, "Please retry"),           // deadlock victim
                _ when sql is { Number: 2601 or 2627 } => (StatusCodes.Status409Conflict, "Duplicate value"), // unique index
                _ => (StatusCodes.Status500InternalServerError, "Server error")
            };

            if (status >= 500) _logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);
            else _logger.LogWarning("{Title} on {Path}: {Message}", title, context.Request.Path, ex.Message);

            var detail = status switch
            {
                < 500 when sql is { Number: 1205 } => "The system was busy. Please try again.",
                < 500 when sql is { Number: 2601 or 2627 } => "A record with the same unique value already exists.",
                < 500 => ex.Message,
                _ => _env.IsDevelopment() ? ex.ToString() : "An unexpected error occurred."
            };

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };
            problem.Extensions["traceId"] = context.TraceIdentifier;

            context.Response.Clear();
            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
        }
    }
}

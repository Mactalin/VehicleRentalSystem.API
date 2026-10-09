using VehicleRentalSystem.Application.Interface_Service;

namespace VehicleRentalSystem.BackgroundServices
{
    public class BookingExpiryWorker : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

        private readonly IServiceScopeFactory _scopes;
        private readonly ILogger<BookingExpiryWorker> _logger;

        public BookingExpiryWorker(IServiceScopeFactory scopes, ILogger<BookingExpiryWorker> logger)
        {
            _scopes = scopes;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    try
                    {
                        // Services are scoped, so each run gets its own scope (and its own DbContext)
                        using var scope = _scopes.CreateScope();
                        var bookings = scope.ServiceProvider.GetRequiredService<IBookingService>();

                        var expired = await bookings.ExpireStaleBookingsAsync(stoppingToken);
                        if (expired > 0)
                            _logger.LogInformation("Expired {Count} unpaid booking(s).", expired);
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        _logger.LogError(ex, "Booking expiry run failed. Will retry on the next tick.");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // App is shutting down.
            }
        }
    }
}

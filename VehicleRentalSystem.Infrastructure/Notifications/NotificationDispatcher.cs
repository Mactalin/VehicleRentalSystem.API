using Microsoft.Extensions.Logging;
using VehicleRentalSystem.Application.DTOs.Notifications;
using VehicleRentalSystem.Application.Interface_Service;

namespace VehicleRentalSystem.Infrastructure.Notifications;

public class NotificationDispatcher : INotificationDispatcher
{
    private readonly IEnumerable<INotificationChannel> _channels;
    private readonly ILogger<NotificationDispatcher> _logger;

    public NotificationDispatcher(IEnumerable<INotificationChannel> channels, ILogger<NotificationDispatcher> logger)
    {
        _channels = channels;
        _logger = logger;
    }

    public async Task DispatchAsync(NotificationRequest request, CancellationToken ct = default)
    {
        foreach (var channel in _channels)
        {
            try
            {
                await channel.SendAsync(request, ct);
            }
            catch (Exception ex)
            {
                // One channel failing shouldn't stop others. Errors are already logged inside channels.
                _logger.LogWarning(ex, "Channel {Channel} failed", channel.GetType().Name);
            }
        }
    }
}
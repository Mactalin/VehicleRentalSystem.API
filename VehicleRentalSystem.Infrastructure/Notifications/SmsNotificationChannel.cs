using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using VehicleRentalSystem.Application.DTOs.Notifications;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Infrastructure.Settings;

namespace VehicleRentalSystem.Infrastructure.Notifications;

public class SmsNotificationChannel : INotificationChannel
{
    private readonly SmsSettings _settings;
    private readonly ILogger<SmsNotificationChannel> _logger;

    public SmsNotificationChannel(IOptions<SmsSettings> settings, ILogger<SmsNotificationChannel> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendAsync(NotificationRequest request, CancellationToken ct = default)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation("[SMS DISABLED] → {Phone}: {Body}", request.ToPhone, request.Body);
            return;
        }

        if (!request.SendSms || string.IsNullOrWhiteSpace(request.ToPhone)) return;

        try
        {
            TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);

            await MessageResource.CreateAsync(
                body: request.Body,
                from: new PhoneNumber(_settings.FromNumber),
                to: new PhoneNumber(request.ToPhone));

            _logger.LogInformation("📱 SMS sent → {Phone}", request.ToPhone);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ SMS send failed → {Phone}", request.ToPhone);
            throw;
        }
    }
}
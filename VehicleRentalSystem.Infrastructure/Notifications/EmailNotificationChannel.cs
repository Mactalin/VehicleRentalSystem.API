using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;
using VehicleRentalSystem.Application.DTOs.Notifications;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Infrastructure.Settings;

namespace VehicleRentalSystem.Infrastructure.Notifications;

public class EmailNotificationChannel : INotificationChannel
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailNotificationChannel> _logger;

    public EmailNotificationChannel(IOptions<EmailSettings> settings, ILogger<EmailNotificationChannel> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendAsync(NotificationRequest request, CancellationToken ct = default)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation("[Email DISABLED] → {Email}: {Subject}", request.ToEmail, request.Subject);
            return;
        }

        if (!request.SendEmail || string.IsNullOrWhiteSpace(request.ToEmail)) return;

        try
        {
            var html = EmailTemplateBuilder.Build(
                evt: request.Event,
                recipientName: ExtractName(request.ToEmail),
                message: request.Body,
                actionUrl: request.ActionUrl,
                actionLabel: request.ActionLabel);

            var text = EmailTemplateBuilder.BuildPlainText(request.Body, request.Subject);

            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
            msg.To.Add(new MailboxAddress("", request.ToEmail));
            msg.Subject = request.Subject;
            msg.Body = new BodyBuilder { TextBody = text, HtmlBody = html }.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(_settings.SmtpServer, _settings.Port,
                MailKit.Security.SecureSocketOptions.StartTls, ct);
            await client.AuthenticateAsync(_settings.Username, _settings.Password, ct);
            await client.SendAsync(msg, ct);
            await client.DisconnectAsync(true, ct);

            _logger.LogInformation("📧 Email sent → {Email}", request.ToEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Email send failed → {Email}", request.ToEmail);
            throw;
        }
    }

    private static string ExtractName(string email)
    {
        var local = email.Split('@')[0].Replace('.', ' ').Replace('_', ' ');
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(local);
    }
}
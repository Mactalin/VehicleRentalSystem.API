using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Infrastructure.Notifications;

public static class EmailTemplateBuilder
{
    public static string Build(
        NotificationEvent evt,
        string recipientName,
        string message,
        string? actionUrl = null,
        string? actionLabel = null)
    {
        var (title, accent, emoji) = evt switch
        {
            NotificationEvent.BookingConfirmation => ("Booking Confirmed", "#10b981", "✅"),
            NotificationEvent.PaymentConfirmation => ("Payment Received", "#3b82f6", "💳"),
            NotificationEvent.PickupReminder => ("Pickup Reminder", "#f59e0b", "🚗"),
            NotificationEvent.ReturnReminder => ("Return Reminder", "#f59e0b", "⏰"),
            NotificationEvent.Cancellation => ("Booking Cancelled", "#ef4444", "❌"),
            NotificationEvent.ReviewRequest => ("How was your trip?", "#8b5cf6", "⭐"),
            NotificationEvent.ComplaintResponse => ("Complaint Update", "#6b7280", "📣"),
            _ => ("Notification", "#6b7280", "🔔")
        };

        var button = (!string.IsNullOrWhiteSpace(actionUrl) && !string.IsNullOrWhiteSpace(actionLabel))
            ? $"""
                <tr><td style="padding:8px 40px 32px 40px;text-align:center;">
                  <a href="{actionUrl}" style="display:inline-block;background:{accent};color:#fff;
                     text-decoration:none;padding:14px 32px;border-radius:8px;font-weight:600;">
                    {actionLabel}
                  </a>
                </td></tr>
              """
            : "";

        return $"""
        <!DOCTYPE html>
        <html><head><meta charset="utf-8" /><title>{title}</title></head>
        <body style="margin:0;padding:0;background:#f3f4f6;font-family:'Segoe UI',Roboto,Helvetica,Arial,sans-serif;color:#111827;">
          <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="padding:32px 16px;">
            <tr><td align="center">
              <table role="presentation" width="600" cellpadding="0" cellspacing="0"
                     style="max-width:600px;background:#fff;border-radius:12px;overflow:hidden;box-shadow:0 4px 12px rgba(0,0,0,0.06);">
                <tr><td style="background:{accent};padding:20px 40px;color:#fff;font-size:20px;font-weight:700;">
                  🚗 Vehicle Rental System
                </td></tr>
                <tr><td style="padding:32px 40px 8px 40px;">
                  <h1 style="margin:0;font-size:24px;font-weight:700;color:{accent};">{emoji} {title}</h1>
                </td></tr>
                <tr><td style="padding:16px 40px 0 40px;font-size:15px;color:#374151;">
                  Hi <strong>{recipientName}</strong>,
                </td></tr>
                <tr><td style="padding:12px 40px 24px 40px;font-size:15px;line-height:1.6;color:#4b5563;">
                  {message}
                </td></tr>
                {button}
                <tr><td style="padding:0 40px;"><hr style="border:none;border-top:1px solid #e5e7eb;margin:0;" /></td></tr>
                <tr><td style="padding:20px 40px;font-size:13px;color:#6b7280;">
                  Need help? Reply to this email or contact
                  <a href="mailto:support@vehiclerental.com" style="color:{accent};text-decoration:none;">support@vehiclerental.com</a>
                </td></tr>
                <tr><td style="background:#f9fafb;padding:20px 40px;font-size:12px;color:#9ca3af;text-align:center;border-top:1px solid #e5e7eb;">
                  © {DateTime.UtcNow.Year} Vehicle Rental System. All rights reserved.
                </td></tr>
              </table>
            </td></tr>
          </table>
        </body></html>
        """;
    }

    public static string BuildPlainText(string message, string title)
        => $"""
           Vehicle Rental System
           ---------------------

           {title}

           {message}

           ---
           support@vehiclerental.com
           © {DateTime.UtcNow.Year} Vehicle Rental System
           """;
}
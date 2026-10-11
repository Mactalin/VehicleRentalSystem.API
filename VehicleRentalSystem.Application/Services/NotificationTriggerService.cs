using VehicleRentalSystem.Application.DTOs.Notifications;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Services;

public class NotificationTriggerService : INotificationTriggerService
{
    private readonly INotificationDispatcher _dispatcher;

    public NotificationTriggerService(INotificationDispatcher dispatcher) => _dispatcher = dispatcher;

    public async Task TriggerAsync(
        int userId,
        string? toEmail,
        string? toPhone,
        NotificationEvent evt,
        string? customMessage = null,
        string? actionUrl = null,
        string? actionLabel = null,
        CancellationToken ct = default)
    {
        var (subject, body, sendEmail, sendSms) = evt switch
        {
            NotificationEvent.BookingConfirmation =>
                ("Booking Confirmed", "Your booking has been confirmed. Thank you for choosing us!", true, true),
            NotificationEvent.PaymentConfirmation =>
                ("Payment Received", "We've successfully received your payment. Thank you!", true, true),
            NotificationEvent.PickupReminder =>
                ("Pickup Reminder", "Your vehicle pickup is scheduled soon. Please be ready!", false, true),
            NotificationEvent.ReturnReminder =>
                ("Return Reminder", "Please remember to return the vehicle on time.", false, true),
            NotificationEvent.Cancellation =>
                ("Booking Cancelled", "Your booking has been cancelled. If this wasn't you, contact support.", true, true),
            NotificationEvent.ReviewRequest =>
                ("How was your trip?", "We'd love to hear your feedback. Please leave a review.", true, false),
            NotificationEvent.ComplaintResponse =>
                ("Complaint Update", "There's an update on your complaint.", true, false),
            _ => ("Notification", "You have a new notification.", true, false)
        };

        var request = new NotificationRequest
        {
            UserId = userId,
            ToEmail = toEmail ?? string.Empty,
            ToPhone = toPhone ?? string.Empty,
            Subject = subject,
            Body = customMessage ?? body,
            Event = evt,
            SendEmail = sendEmail,
            SendSms = sendSms,
            ActionUrl = actionUrl,
            ActionLabel = actionLabel
        };

        await _dispatcher.DispatchAsync(request, ct);
    }
}
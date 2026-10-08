using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.ServiceInterfaces;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.RepositoryInterfaces;

namespace VehicleRentalSystem.Application.Services;

public class NotificationTriggerService : INotificationTriggerService
{
    private readonly INotificationRepository _repo;

    public NotificationTriggerService(INotificationRepository repo) => _repo = repo;

    public async Task TriggerAsync(int userId, NotificationEvent evt, string? customMessage = null)
    {
        var (title, defaultMessage, type) = evt switch
        {
            NotificationEvent.BookingConfirmation => ("Booking Confirmed", "Your booking has been confirmed.", NotificationType.Email),
            NotificationEvent.PaymentConfirmation => ("Payment Received", "We received your payment. Thank you!", NotificationType.Email),
            NotificationEvent.PickupReminder => ("Pickup Reminder", "Your vehicle pickup is due soon.", NotificationType.SMS),
            NotificationEvent.ReturnReminder => ("Return Reminder", "Please return your vehicle on time.", NotificationType.SMS),
            NotificationEvent.Cancellation => ("Booking Cancelled", "Your booking has been cancelled.", NotificationType.Email),
            _ => ("Notification", "You have a new notification.", NotificationType.InApp)
        };

        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = customMessage ?? defaultMessage,
            Type = type,
            Event = evt,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await _repo.AddAsync(notification);
    }
}

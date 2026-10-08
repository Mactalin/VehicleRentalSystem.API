using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Domain.Enums;

public enum DiscountType
{
    Percentage,
    Fixed
}

public enum ServiceType
{
    GPS,
    ChildSeat,
    ExtraInsurance,
    Accessory,
    Other
}

public enum ComplaintStatus
{
    Pending,
    InProgress,
    Resolved,
    Closed
}

public enum NotificationType
{
    Email,
    SMS,
    InApp
}

public enum NotificationEvent
{
    BookingConfirmation,
    PaymentConfirmation,
    PickupReminder,
    ReturnReminder,
    Cancellation
}
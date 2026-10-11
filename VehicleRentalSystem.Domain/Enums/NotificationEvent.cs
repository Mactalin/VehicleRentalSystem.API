namespace VehicleRentalSystem.Domain.Enums
{
    public enum NotificationEvent
    {
        BookingConfirmation = 1,
        PaymentConfirmation = 2,
        PickupReminder = 3,
        ReturnReminder = 4,
        Cancellation = 5,
        ReviewRequest = 6,
        ComplaintResponse = 7
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.ServiceInterfaces;

public interface INotificationTriggerService
{
    Task TriggerAsync(int userId, NotificationEvent evt, string? customMessage = null);
}

using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.DTOs;

namespace VehicleRentalSystem.Application.ServiceInterfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetAllAsync();
    Task<NotificationDto?> GetByIdAsync(int id);
    Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId);
    Task<IEnumerable<NotificationDto>> GetUnreadByUserIdAsync(int userId);
    Task<NotificationDto> CreateAsync(CreateNotificationDto dto);
    Task MarkAsReadAsync(int id);
    Task DeleteAsync(int id);
}

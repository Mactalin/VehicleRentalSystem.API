using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using VehicleRentalSystem.Application.DTOs;
using VehicleRentalSystem.Application.ServiceInterfaces;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.RepositoryInterfaces;

namespace VehicleRentalSystem.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly IMapper _mapper;

    public NotificationService(INotificationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationDto>> GetAllAsync()
        => _mapper.Map<IEnumerable<NotificationDto>>(await _repository.GetAllAsync());

    public async Task<NotificationDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<NotificationDto>(entity);
    }

    public async Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId)
        => _mapper.Map<IEnumerable<NotificationDto>>(await _repository.GetByUserIdAsync(userId));

    public async Task<IEnumerable<NotificationDto>> GetUnreadByUserIdAsync(int userId)
        => _mapper.Map<IEnumerable<NotificationDto>>(await _repository.GetUnreadByUserIdAsync(userId));

    public async Task<NotificationDto> CreateAsync(CreateNotificationDto dto)
    {
        var entity = _mapper.Map<Notification>(dto);
        entity.CreatedAt = DateTime.UtcNow;
        entity.IsRead = false;
        await _repository.AddAsync(entity);
        return _mapper.Map<NotificationDto>(entity);
    }

    public async Task MarkAsReadAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Notification not found");
        entity.IsRead = true;
        await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Notification not found");
        await _repository.DeleteAsync(entity);
    }
}
using AutoMapper;
using VehicleRentalSystem.Application.DTOs.Notifications;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Exceptions;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repo;
    private readonly IMapper _mapper;

    public NotificationService(INotificationRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<IEnumerable<NotificationDto>>(await _repo.GetAllAsync(ct));

    public async Task<NotificationDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var n = await _repo.GetByIdAsync(id, ct);
        return n is null ? null : _mapper.Map<NotificationDto>(n);
    }

    public async Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<NotificationDto>>(await _repo.GetByUserIdAsync(userId, ct));

    public async Task<IEnumerable<NotificationDto>> GetUnreadByUserIdAsync(int userId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<NotificationDto>>(await _repo.GetUnreadByUserIdAsync(userId, ct));

    public async Task<int> GetUnreadCountAsync(int userId, CancellationToken ct = default)
        => await _repo.GetUnreadCountAsync(userId, ct);

    public async Task<NotificationDto> CreateAsync(CreateNotificationDto dto, CancellationToken ct = default)
    {
        var n = _mapper.Map<Notification>(dto);
        n.IsRead = false;
        n.SentAt = DateTime.UtcNow;

        await _repo.AddAsync(n, ct);
        await _repo.SaveChangesAsync(ct);
        return _mapper.Map<NotificationDto>(n);
    }

    public async Task MarkAsReadAsync(int id, CancellationToken ct = default)
    {
        var n = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Notification), id);
        n.MarkRead();
        _repo.Update(n);
        await _repo.SaveChangesAsync(ct);
    }

    public async Task MarkAllAsReadAsync(int userId, CancellationToken ct = default)
    {
        var unread = await _repo.GetUnreadByUserIdAsync(userId, ct);
        foreach (var n in unread)
        {
            n.MarkRead();
            _repo.Update(n);
        }
        await _repo.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var n = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Notification), id);
        _repo.Delete(n);
        await _repo.SaveChangesAsync(ct);
    }
}
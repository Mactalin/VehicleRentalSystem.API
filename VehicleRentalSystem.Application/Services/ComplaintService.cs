using AutoMapper;
using VehicleRentalSystem.Application.DTOs.Complaints;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Exceptions;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services;

public class ComplaintService : IComplaintService
{
    private readonly IComplaintRepository _repo;
    private readonly INotificationTriggerService _notifier;
    private readonly IMapper _mapper;

    public ComplaintService(
        IComplaintRepository repo,
        INotificationTriggerService notifier,
        IMapper mapper)
    {
        _repo = repo;
        _notifier = notifier;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ComplaintDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<IEnumerable<ComplaintDto>>(await _repo.GetAllAsync(ct));

    public async Task<ComplaintDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var c = await _repo.GetByIdAsync(id, ct);
        return c is null ? null : _mapper.Map<ComplaintDto>(c);
    }

    public async Task<IEnumerable<ComplaintDto>> GetByStatusAsync(ComplaintStatus status, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<ComplaintDto>>(await _repo.GetByStatusAsync(status, ct));

    public async Task<IEnumerable<ComplaintDto>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<ComplaintDto>>(await _repo.GetByCustomerIdAsync(customerId, ct));

    public async Task<ComplaintDto> CreateAsync(CreateComplaintDto dto, CancellationToken ct = default)
    {
        var complaint = _mapper.Map<Complaint>(dto);
        complaint.Status = ComplaintStatus.Pending;

        await _repo.AddAsync(complaint, ct);
        await _repo.SaveChangesAsync(ct);
        return _mapper.Map<ComplaintDto>(complaint);
    }

    public async Task<ComplaintDto> RespondAsync(int id, RespondToComplaintDto dto, CancellationToken ct = default)
    {
        var complaint = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Complaint), id);

        complaint.Respond(dto.AdminResponse, dto.AdminUserId);
        _repo.Update(complaint);
        await _repo.SaveChangesAsync(ct);

        // Best-effort notification (won't fail the request if email/SMS fails)
        _ = SafeNotifyAsync(complaint, ct);

        return _mapper.Map<ComplaintDto>(complaint);
    }

    public async Task<ComplaintDto> ResolveAsync(int id, CancellationToken ct = default)
    {
        var complaint = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Complaint), id);

        complaint.Resolve();
        _repo.Update(complaint);
        await _repo.SaveChangesAsync(ct);
        return _mapper.Map<ComplaintDto>(complaint);
    }

    public async Task<ComplaintDto> CloseAsync(int id, CancellationToken ct = default)
    {
        var complaint = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Complaint), id);

        complaint.Close();
        _repo.Update(complaint);
        await _repo.SaveChangesAsync(ct);
        return _mapper.Map<ComplaintDto>(complaint);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var complaint = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Complaint), id);
        _repo.Delete(complaint);
        await _repo.SaveChangesAsync(ct);
    }

    private async Task SafeNotifyAsync(Complaint complaint, CancellationToken ct)
    {
        try
        {
            await _notifier.TriggerAsync(
                userId: complaint.CustomerId,
                toEmail: null,
                toPhone: null,
                evt: NotificationEvent.ComplaintResponse,
                customMessage: $"Your complaint #{complaint.Id} has been updated: {complaint.AdminResponse}",
                ct: ct);
        }
        catch
        {
            // ignored — notification failure must not break the admin flow
        }
    }
}
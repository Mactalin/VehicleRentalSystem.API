using VehicleRentalSystem.Application.DTOs.Complaints;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Interface_Service;

public interface IComplaintService
{
    Task<IEnumerable<ComplaintDto>> GetAllAsync(CancellationToken ct = default);
    Task<ComplaintDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<ComplaintDto>> GetByStatusAsync(ComplaintStatus status, CancellationToken ct = default);
    Task<IEnumerable<ComplaintDto>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default);
    Task<ComplaintDto> CreateAsync(CreateComplaintDto dto, CancellationToken ct = default);
    Task<ComplaintDto> RespondAsync(int id, RespondToComplaintDto dto, CancellationToken ct = default);
    Task<ComplaintDto> ResolveAsync(int id, CancellationToken ct = default);
    Task<ComplaintDto> CloseAsync(int id, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
using VehicleRentalSystem.Application.DTOs.AdditionalServices;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Interface_Service;

public interface IAdditionalServiceService
{
    Task<IEnumerable<AdditionalServiceDto>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<AdditionalServiceDto>> GetActiveAsync(CancellationToken ct = default);
    Task<AdditionalServiceDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<AdditionalServiceDto>> GetByTypeAsync(ServiceType type, CancellationToken ct = default);
    Task<AdditionalServiceDto> CreateAsync(CreateAdditionalServiceDto dto, CancellationToken ct = default);
    Task<AdditionalServiceDto> UpdateAsync(int id, UpdateAdditionalServiceDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
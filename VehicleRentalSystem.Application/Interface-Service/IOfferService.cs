using VehicleRentalSystem.Application.DTOs.Offers;

namespace VehicleRentalSystem.Application.Interface_Service;

public interface IOfferService
{
    Task<IEnumerable<OfferDto>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<OfferDto>> GetRunningAsync(CancellationToken ct = default);
    Task<OfferDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<OfferDto> CreateAsync(CreateOfferDto dto, CancellationToken ct = default);
    Task<OfferDto> UpdateAsync(int id, UpdateOfferDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
using VehicleRentalSystem.Application.DTOs.Drivers;

namespace VehicleRentalSystem.Application.Interface_Service;

public interface IDriverService
{
    Task<IEnumerable<DriverDto>> GetAllAsync(CancellationToken ct = default);
    Task<DriverDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<DriverDto>> GetAvailableAsync(CancellationToken ct = default);
    Task<DriverDto> CreateAsync(CreateDriverDto dto, CancellationToken ct = default);
    Task<DriverDto> UpdateAsync(int id, UpdateDriverDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
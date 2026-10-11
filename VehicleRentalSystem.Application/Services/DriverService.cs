using AutoMapper;
using VehicleRentalSystem.Application.DTOs.Drivers;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Exceptions;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services;

public class DriverService : IDriverService
{
    private readonly IDriverRepository _repo;
    private readonly IMapper _mapper;

    public DriverService(IDriverRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DriverDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<IEnumerable<DriverDto>>(await _repo.GetAllAsync(ct));

    public async Task<DriverDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var d = await _repo.GetByIdAsync(id, ct);
        return d is null ? null : _mapper.Map<DriverDto>(d);
    }

    public async Task<IEnumerable<DriverDto>> GetAvailableAsync(CancellationToken ct = default)
        => _mapper.Map<IEnumerable<DriverDto>>(await _repo.GetAvailableAsync(ct));

    public async Task<DriverDto> CreateAsync(CreateDriverDto dto, CancellationToken ct = default)
    {
        var existing = await _repo.GetByLicenseNumberAsync(dto.LicenseNumber, ct);
        if (existing is not null)
            throw new DomainException($"A driver with license '{dto.LicenseNumber}' already exists.");

        var driver = _mapper.Map<Driver>(dto);
        await _repo.AddAsync(driver, ct);
        await _repo.SaveChangesAsync(ct);
        return _mapper.Map<DriverDto>(driver);
    }

    public async Task<DriverDto> UpdateAsync(int id, UpdateDriverDto dto, CancellationToken ct = default)
    {
        var driver = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Driver), id);

        _mapper.Map(dto, driver);
        _repo.Update(driver);
        await _repo.SaveChangesAsync(ct);
        return _mapper.Map<DriverDto>(driver);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var driver = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Driver), id);

        if (await _repo.HasActiveAssignmentAsync(id, ct))
            throw new DomainException("Cannot delete a driver with active assignments.");

        _repo.Delete(driver);   // soft delete via BaseEntity
        await _repo.SaveChangesAsync(ct);
    }
}
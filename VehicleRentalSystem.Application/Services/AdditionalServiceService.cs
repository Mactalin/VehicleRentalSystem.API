using AutoMapper;
using VehicleRentalSystem.Application.DTOs.AdditionalServices;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Exceptions;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services;

public class AdditionalServiceService : IAdditionalServiceService
{
    private readonly IAdditionalServiceRepository _repo;
    private readonly IMapper _mapper;

    public AdditionalServiceService(IAdditionalServiceRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AdditionalServiceDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<IEnumerable<AdditionalServiceDto>>(await _repo.GetAllAsync(ct));

    public async Task<IEnumerable<AdditionalServiceDto>> GetActiveAsync(CancellationToken ct = default)
        => _mapper.Map<IEnumerable<AdditionalServiceDto>>(await _repo.GetActiveAsync(ct));

    public async Task<AdditionalServiceDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var s = await _repo.GetByIdAsync(id, ct);
        return s is null ? null : _mapper.Map<AdditionalServiceDto>(s);
    }

    public async Task<IEnumerable<AdditionalServiceDto>> GetByTypeAsync(ServiceType type, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<AdditionalServiceDto>>(await _repo.GetByTypeAsync(type, ct));

    public async Task<AdditionalServiceDto> CreateAsync(CreateAdditionalServiceDto dto, CancellationToken ct = default)
    {
        var svc = _mapper.Map<AdditionalService>(dto);
        await _repo.AddAsync(svc, ct);
        await _repo.SaveChangesAsync(ct);
        return _mapper.Map<AdditionalServiceDto>(svc);
    }

    public async Task<AdditionalServiceDto> UpdateAsync(int id, UpdateAdditionalServiceDto dto, CancellationToken ct = default)
    {
        var svc = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(AdditionalService), id);

        _mapper.Map(dto, svc);
        _repo.Update(svc);
        await _repo.SaveChangesAsync(ct);
        return _mapper.Map<AdditionalServiceDto>(svc);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var svc = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(AdditionalService), id);
        _repo.Delete(svc);
        await _repo.SaveChangesAsync(ct);
    }
}
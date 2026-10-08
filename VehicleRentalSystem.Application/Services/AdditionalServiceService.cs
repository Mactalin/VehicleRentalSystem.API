using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using VehicleRentalSystem.Application.DTOs;
using VehicleRentalSystem.Application.ServiceInterfaces;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.RepositoryInterfaces;

namespace VehicleRentalSystem.Application.Services;

public class AdditionalServiceService : IAdditionalServiceService
{
    private readonly IAdditionalServiceRepository _repository;
    private readonly IMapper _mapper;

    public AdditionalServiceService(IAdditionalServiceRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AdditionalServiceDto>> GetAllAsync()
        => _mapper.Map<IEnumerable<AdditionalServiceDto>>(await _repository.GetAllAsync());

    public async Task<AdditionalServiceDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<AdditionalServiceDto>(entity);
    }

    public async Task<IEnumerable<AdditionalServiceDto>> GetByTypeAsync(ServiceType type)
        => _mapper.Map<IEnumerable<AdditionalServiceDto>>(await _repository.GetByTypeAsync(type));

    public async Task<AdditionalServiceDto> CreateAsync(CreateAdditionalServiceDto dto)
    {
        var entity = _mapper.Map<AdditionalService>(dto);
        await _repository.AddAsync(entity);
        return _mapper.Map<AdditionalServiceDto>(entity);
    }

    public async Task<AdditionalServiceDto> UpdateAsync(int id, CreateAdditionalServiceDto dto)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Additional service not found");
        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity);
        return _mapper.Map<AdditionalServiceDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Additional service not found");
        await _repository.DeleteAsync(entity);
    }
}
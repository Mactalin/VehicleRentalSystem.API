using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using VehicleRentalSystem.Application.DTOs;
using VehicleRentalSystem.Application.ServiceInterfaces;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.RepositoryInterfaces;

namespace VehicleRentalSystem.Application.Services;

public class OfferService : IOfferService
{
    private readonly IOfferRepository _repository;
    private readonly IMapper _mapper;

    public OfferService(IOfferRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OfferDto>> GetAllAsync()
        => _mapper.Map<IEnumerable<OfferDto>>(await _repository.GetAllAsync());

    public async Task<OfferDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<OfferDto>(entity);
    }

    public async Task<IEnumerable<OfferDto>> GetActiveOffersAsync()
        => _mapper.Map<IEnumerable<OfferDto>>(await _repository.GetActiveOffersAsync());

    public async Task<OfferDto> CreateAsync(CreateOfferDto dto)
    {
        var entity = _mapper.Map<Offer>(dto);
        entity.IsActive = true;
        await _repository.AddAsync(entity);
        return _mapper.Map<OfferDto>(entity);
    }

    public async Task<OfferDto> UpdateAsync(int id, CreateOfferDto dto)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Offer not found");
        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity);
        return _mapper.Map<OfferDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Offer not found");
        await _repository.DeleteAsync(entity);
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using VehicleRentalSystem.Application.DTOs;
using VehicleRentalSystem.Application.ServiceInterfaces;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.RepositoryInterfaces;

namespace VehicleRentalSystem.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _repository;
    private readonly IMapper _mapper;

    public ReviewService(IReviewRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReviewDto>> GetAllAsync()
        => _mapper.Map<IEnumerable<ReviewDto>>(await _repository.GetAllAsync());

    public async Task<ReviewDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<ReviewDto>(entity);
    }

    public async Task<IEnumerable<ReviewDto>> GetByVehicleIdAsync(int vehicleId)
        => _mapper.Map<IEnumerable<ReviewDto>>(await _repository.GetByVehicleIdAsync(vehicleId));

    public async Task<IEnumerable<ReviewDto>> GetByDriverIdAsync(int driverId)
        => _mapper.Map<IEnumerable<ReviewDto>>(await _repository.GetByDriverIdAsync(driverId));

    public async Task<IEnumerable<ReviewDto>> GetByCustomerIdAsync(int customerId)
        => _mapper.Map<IEnumerable<ReviewDto>>(await _repository.GetByCustomerIdAsync(customerId));

    public async Task<ReviewDto> CreateAsync(CreateReviewDto dto)
    {
        var entity = _mapper.Map<Review>(dto);
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity);
        return _mapper.Map<ReviewDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Review not found");
        await _repository.DeleteAsync(entity);
    }
}
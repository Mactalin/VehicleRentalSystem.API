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

public class ComplaintService : IComplaintService
{
    private readonly IComplaintRepository _repository;
    private readonly IMapper _mapper;

    public ComplaintService(IComplaintRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ComplaintDto>> GetAllAsync()
        => _mapper.Map<IEnumerable<ComplaintDto>>(await _repository.GetAllAsync());

    public async Task<ComplaintDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<ComplaintDto>(entity);
    }

    public async Task<IEnumerable<ComplaintDto>> GetByStatusAsync(ComplaintStatus status)
        => _mapper.Map<IEnumerable<ComplaintDto>>(await _repository.GetByStatusAsync(status));

    public async Task<IEnumerable<ComplaintDto>> GetByCustomerIdAsync(int customerId)
        => _mapper.Map<IEnumerable<ComplaintDto>>(await _repository.GetByCustomerIdAsync(customerId));

    public async Task<ComplaintDto> CreateAsync(CreateComplaintDto dto)
    {
        var entity = _mapper.Map<Complaint>(dto);
        entity.Status = ComplaintStatus.Pending;
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity);
        return _mapper.Map<ComplaintDto>(entity);
    }

    public async Task<ComplaintDto> UpdateStatusAsync(int id, UpdateComplaintStatusDto dto)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Complaint not found");
        entity.Status = dto.Status;
        entity.AdminResponse = dto.AdminResponse;
        if (dto.Status == ComplaintStatus.Resolved || dto.Status == ComplaintStatus.Closed)
            entity.ResolvedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(entity);
        return _mapper.Map<ComplaintDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Complaint not found");
        await _repository.DeleteAsync(entity);
    }
}
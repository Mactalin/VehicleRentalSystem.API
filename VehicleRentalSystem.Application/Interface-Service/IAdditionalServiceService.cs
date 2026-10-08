using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.DTOs;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.ServiceInterfaces;

public interface IAdditionalServiceService
{
    Task<IEnumerable<AdditionalServiceDto>> GetAllAsync();
    Task<AdditionalServiceDto?> GetByIdAsync(int id);
    Task<IEnumerable<AdditionalServiceDto>> GetByTypeAsync(ServiceType type);
    Task<AdditionalServiceDto> CreateAsync(CreateAdditionalServiceDto dto);
    Task<AdditionalServiceDto> UpdateAsync(int id, CreateAdditionalServiceDto dto);
    Task DeleteAsync(int id);
}
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.DTOs;

namespace VehicleRentalSystem.Application.ServiceInterfaces;

public interface IDriverService
{
    Task<IEnumerable<DriverDto>> GetAllDriversAsync();
    Task<DriverDto?> GetDriverByIdAsync(int id);
    Task<DriverDto> CreateDriverAsync(CreateDriverDto dto);
    Task<DriverDto> UpdateDriverAsync(int id, UpdateDriverDto dto);
    Task DeleteDriverAsync(int id);
    Task<IEnumerable<DriverDto>> GetAvailableDriversAsync();
}
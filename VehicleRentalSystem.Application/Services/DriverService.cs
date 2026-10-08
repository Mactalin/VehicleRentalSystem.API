using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using VehicleRentalSystem.Application.DTOs;
using VehicleRentalSystem.Application.ServiceInterfaces;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.RepositoryInterfaces;

namespace VehicleRentalSystem.Application.Services;

public class DriverService : IDriverService
{
    private readonly IDriverRepository _driverRepository;
    private readonly IMapper _mapper;

    public DriverService(IDriverRepository driverRepository, IMapper mapper)
    {
        _driverRepository = driverRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DriverDto>> GetAllDriversAsync()
    {
        var drivers = await _driverRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<DriverDto>>(drivers);
    }

    public async Task<DriverDto?> GetDriverByIdAsync(int id)
    {
        var driver = await _driverRepository.GetByIdAsync(id);
        return driver == null ? null : _mapper.Map<DriverDto>(driver);
    }

    public async Task<DriverDto> CreateDriverAsync(CreateDriverDto dto)
    {
        var driver = _mapper.Map<Driver>(dto);
        await _driverRepository.AddAsync(driver);
        return _mapper.Map<DriverDto>(driver);
    }

    public async Task<DriverDto> UpdateDriverAsync(int id, UpdateDriverDto dto)
    {
        var driver = await _driverRepository.GetByIdAsync(id);
        if (driver == null) throw new KeyNotFoundException("Driver not found");

        _mapper.Map(dto, driver);
        await _driverRepository.UpdateAsync(driver);
        return _mapper.Map<DriverDto>(driver);
    }

    public async Task DeleteDriverAsync(int id)
    {
        var driver = await _driverRepository.GetByIdAsync(id);
        if (driver == null) throw new KeyNotFoundException("Driver not found");
        await _driverRepository.DeleteAsync(driver);
    }

    public async Task<IEnumerable<DriverDto>> GetAvailableDriversAsync()
    {
        var drivers = await _driverRepository.GetAvailableDriversAsync();
        return _mapper.Map<IEnumerable<DriverDto>>(drivers);
    }
}
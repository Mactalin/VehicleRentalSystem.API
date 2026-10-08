using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.DTOs;

namespace VehicleRentalSystem.Application.ServiceInterfaces;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetAllAsync();
    Task<ReviewDto?> GetByIdAsync(int id);
    Task<IEnumerable<ReviewDto>> GetByVehicleIdAsync(int vehicleId);
    Task<IEnumerable<ReviewDto>> GetByDriverIdAsync(int driverId);
    Task<IEnumerable<ReviewDto>> GetByCustomerIdAsync(int customerId);
    Task<ReviewDto> CreateAsync(CreateReviewDto dto);
    Task DeleteAsync(int id);
}
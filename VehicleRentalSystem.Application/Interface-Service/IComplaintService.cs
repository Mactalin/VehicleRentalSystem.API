using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.DTOs;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.ServiceInterfaces;

public interface IComplaintService
{
    Task<IEnumerable<ComplaintDto>> GetAllAsync();
    Task<ComplaintDto?> GetByIdAsync(int id);
    Task<IEnumerable<ComplaintDto>> GetByStatusAsync(ComplaintStatus status);
    Task<IEnumerable<ComplaintDto>> GetByCustomerIdAsync(int customerId);
    Task<ComplaintDto> CreateAsync(CreateComplaintDto dto);
    Task<ComplaintDto> UpdateStatusAsync(int id, UpdateComplaintStatusDto dto);
    Task DeleteAsync(int id);
}
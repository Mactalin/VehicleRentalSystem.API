using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Branch;

namespace VehicleRentalSystem.Application.Interface_Service
{
    public interface IBranchService
    {
        Task<IEnumerable<BranchDto>> GetAllAsync(string? city = null);
        Task<BranchDto> GetByIdAsync(int id);
        Task<BranchDto> CreateAsync(SaveBranchDto dto);
        Task<BranchDto> UpdateAsync(int id, SaveBranchDto dto);
        Task DeleteAsync(int id);

        Task<IEnumerable<OperatingHourDto>> GetOperatingHoursAsync(int branchId);
        Task<IEnumerable<OperatingHourDto>> SetOperatingHoursAsync(int branchId, IEnumerable<OperatingHourDto> hours);
    }
}

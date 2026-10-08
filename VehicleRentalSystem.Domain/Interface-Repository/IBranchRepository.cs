using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Interface_Repository
{
    public interface IBranchRepository : IGenericRepository<Branch>
    {
        Task<Branch?> GetByIdWithHoursAsync(int id);
        Task<IEnumerable<Branch>> GetAllWithHoursAsync();
        Task<IEnumerable<Branch>> GetByCityAsync(string city);

        Task<IEnumerable<BranchOperatingHour>> GetOperatingHoursAsync(int branchId);
        Task AddOperatingHoursAsync(IEnumerable<BranchOperatingHour> hours);
        void RemoveOperatingHours(IEnumerable<BranchOperatingHour> hours);
    }
}

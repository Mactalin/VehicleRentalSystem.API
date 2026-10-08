using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.RepositoryInterfaces;

public interface IComplaintRepository : IGenericRepository<Complaint>
{
    Task<IEnumerable<Complaint>> GetByStatusAsync(ComplaintStatus status);
    Task<IEnumerable<Complaint>> GetByCustomerIdAsync(int customerId);
}
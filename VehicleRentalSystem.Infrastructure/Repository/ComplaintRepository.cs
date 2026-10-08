using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.RepositoryInterfaces;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repositories;

public class ComplaintRepository : GenericRepository<Complaint>, IComplaintRepository
{
    public ComplaintRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Complaint>> GetByStatusAsync(ComplaintStatus status)
    {
        return await _dbSet.Where(c => c.Status == status).ToListAsync();
    }

    public async Task<IEnumerable<Complaint>> GetByCustomerIdAsync(int customerId)
    {
        return await _dbSet.Where(c => c.CustomerId == customerId).ToListAsync();
    }
}
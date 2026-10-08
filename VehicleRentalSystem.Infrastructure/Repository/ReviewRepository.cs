using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.RepositoryInterfaces;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repositories;

public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Review>> GetByVehicleIdAsync(int vehicleId)
    {
        return await _dbSet.Where(r => r.VehicleId == vehicleId).ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetByDriverIdAsync(int driverId)
    {
        return await _dbSet.Where(r => r.DriverId == driverId).ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetByCustomerIdAsync(int customerId)
    {
        return await _dbSet.Where(r => r.CustomerId == customerId).ToListAsync();
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.RepositoryInterfaces;

public interface IReviewRepository : IGenericRepository<Review>
{
    Task<IEnumerable<Review>> GetByVehicleIdAsync(int vehicleId);
    Task<IEnumerable<Review>> GetByDriverIdAsync(int driverId);
    Task<IEnumerable<Review>> GetByCustomerIdAsync(int customerId);
}
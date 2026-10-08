using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.RepositoryInterfaces;

public interface IDriverRepository : IGenericRepository<Driver>
{
    Task<IEnumerable<Driver>> GetAvailableDriversAsync();
    Task<Driver?> GetDriverWithBookingsAsync(int id);
}
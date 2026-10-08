using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.RepositoryInterfaces;

public interface IAdditionalServiceRepository : IGenericRepository<AdditionalService>
{
    Task<IEnumerable<AdditionalService>> GetByTypeAsync(Enums.ServiceType type);
}
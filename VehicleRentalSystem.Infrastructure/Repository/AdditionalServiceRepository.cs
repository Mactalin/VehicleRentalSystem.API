using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.RepositoryInterfaces;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repositories;

public class AdditionalServiceRepository : GenericRepository<AdditionalService>, IAdditionalServiceRepository
{
    public AdditionalServiceRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<AdditionalService>> GetByTypeAsync(ServiceType type)
    {
        return await _dbSet.Where(s => s.Type == type).ToListAsync();
    }
}

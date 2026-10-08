using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.RepositoryInterfaces;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repositories;

public class DriverRepository : GenericRepository<Driver>, IDriverRepository
{
    public DriverRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Driver>> GetAvailableDriversAsync()
    {
        return await _dbSet.Where(d => d.IsAvailable).ToListAsync();
    }

    public async Task<Driver?> GetDriverWithBookingsAsync(int id)
    {
        return await _dbSet
            .Include(d => d.BookingDrivers)
                .ThenInclude(bd => bd.Booking)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
}
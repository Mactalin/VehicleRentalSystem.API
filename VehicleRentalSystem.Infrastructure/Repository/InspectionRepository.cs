using Microsoft.EntityFrameworkCore;

using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository
{
    public class InspectionRepository : GenericRepository<VehicleInspection>, IInspectionRepository
    {
        public InspectionRepository(AppDbContext context) : base(context) { }

        public async Task<VehicleInspection?> GetByBookingAndTypeAsync(
            int bookingId, InspectionType type, CancellationToken ct = default) =>
            await _dbSet.FirstOrDefaultAsync(i => i.BookingId == bookingId && i.Type == type, ct);
    }
}

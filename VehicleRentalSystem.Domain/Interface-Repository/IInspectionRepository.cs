using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;


namespace VehicleRentalSystem.Domain.Interface_Repository;

public interface IInspectionRepository : IGenericRepository<VehicleInspection>
{
    Task<VehicleInspection?> GetByBookingAndTypeAsync(int bookingId, InspectionType type, CancellationToken ct = default);
}
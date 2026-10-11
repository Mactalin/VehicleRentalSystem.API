using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Interface_Repository
{
    public interface IComplaintRepository : IGenericRepository<Complaint>
    {
        Task<IEnumerable<Complaint>> GetByStatusAsync(ComplaintStatus status, CancellationToken ct = default);
        Task<IEnumerable<Complaint>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default);
    }
}
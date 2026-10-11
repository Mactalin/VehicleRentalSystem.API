using VehicleRentalSystem.Application.DTOs.Reviews;

namespace VehicleRentalSystem.Application.Interface_Service;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetAllAsync(CancellationToken ct = default);
    Task<ReviewDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<ReviewDto>> GetByVehicleIdAsync(int vehicleId, CancellationToken ct = default);
    Task<IEnumerable<ReviewDto>> GetByDriverIdAsync(int driverId, CancellationToken ct = default);
    Task<IEnumerable<ReviewDto>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default);
    Task<ReviewSummaryDto> GetVehicleSummaryAsync(int vehicleId, CancellationToken ct = default);
    Task<ReviewSummaryDto> GetDriverSummaryAsync(int driverId, CancellationToken ct = default);
    Task<ReviewDto> CreateAsync(CreateReviewDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
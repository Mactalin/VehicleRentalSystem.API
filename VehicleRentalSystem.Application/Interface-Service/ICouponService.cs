using VehicleRentalSystem.Application.DTOs.Coupons;

namespace VehicleRentalSystem.Application.Interface_Service;

public interface ICouponService
{
    Task<IEnumerable<CouponDto>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<CouponDto>> GetActiveAsync(CancellationToken ct = default);
    Task<CouponDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CouponDto?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<CouponDto> CreateAsync(CreateCouponDto dto, CancellationToken ct = default);
    Task<CouponDto> UpdateAsync(int id, CreateCouponDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<CouponValidationResultDto> ValidateAsync(ValidateCouponDto dto, CancellationToken ct = default);
}
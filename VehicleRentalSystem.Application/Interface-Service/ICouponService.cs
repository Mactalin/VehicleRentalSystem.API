using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.DTOs;

namespace VehicleRentalSystem.Application.ServiceInterfaces;

public interface ICouponService
{
    Task<IEnumerable<CouponDto>> GetAllAsync();
    Task<CouponDto?> GetByIdAsync(int id);
    Task<CouponDto?> GetByCodeAsync(string code);
    Task<IEnumerable<CouponDto>> GetActiveCouponsAsync();
    Task<CouponDto> CreateAsync(CreateCouponDto dto);
    Task<CouponDto> UpdateAsync(int id, CreateCouponDto dto);
    Task DeleteAsync(int id);
    Task<CouponValidationResultDto> ValidateAsync(ValidateCouponDto dto);
}

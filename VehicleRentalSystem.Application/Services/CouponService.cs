using AutoMapper;
using VehicleRentalSystem.Application.DTOs.Coupons;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Exceptions;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services;

public class CouponService : ICouponService
{
    private readonly ICouponRepository _repo;
    private readonly IMapper _mapper;

    public CouponService(ICouponRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CouponDto>> GetAllAsync(CancellationToken ct = default)
        => _mapper.Map<IEnumerable<CouponDto>>(await _repo.GetAllAsync(ct));

    public async Task<IEnumerable<CouponDto>> GetActiveAsync(CancellationToken ct = default)
        => _mapper.Map<IEnumerable<CouponDto>>(await _repo.GetActiveAsync(ct));

    public async Task<CouponDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var c = await _repo.GetByIdAsync(id, ct);
        return c is null ? null : _mapper.Map<CouponDto>(c);
    }

    public async Task<CouponDto?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var c = await _repo.GetByCodeAsync(code, ct);
        return c is null ? null : _mapper.Map<CouponDto>(c);
    }

    public async Task<CouponDto> CreateAsync(CreateCouponDto dto, CancellationToken ct = default)
    {
        if (dto.ValidTo <= dto.ValidFrom)
            throw new DomainException("ValidTo must be after ValidFrom.");

        var existing = await _repo.GetByCodeAsync(dto.Code, ct);
        if (existing is not null)
            throw new DomainException($"Coupon code '{dto.Code}' already exists.");

        var coupon = _mapper.Map<Coupon>(dto);
        coupon.Code = coupon.Code.Trim().ToUpperInvariant();
        coupon.Status = CouponStatus.Active;
        coupon.IsActive = true;
        coupon.UsedCount = 0;

        await _repo.AddAsync(coupon, ct);
        await _repo.SaveChangesAsync(ct);
        return _mapper.Map<CouponDto>(coupon);
    }

    public async Task<CouponDto> UpdateAsync(int id, CreateCouponDto dto, CancellationToken ct = default)
    {
        var coupon = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Coupon), id);

        _mapper.Map(dto, coupon);
        coupon.Code = coupon.Code.Trim().ToUpperInvariant();

        _repo.Update(coupon);
        await _repo.SaveChangesAsync(ct);
        return _mapper.Map<CouponDto>(coupon);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var coupon = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Coupon), id);
        _repo.Delete(coupon);
        await _repo.SaveChangesAsync(ct);
    }

    public async Task<CouponValidationResultDto> ValidateAsync(ValidateCouponDto dto, CancellationToken ct = default)
    {
        var coupon = await _repo.GetByCodeAsync(dto.Code.Trim().ToUpperInvariant(), ct);

        if (coupon is null)
            return Fail("Coupon code not found.");

        if (!coupon.IsValidNow())
            return Fail("Coupon is expired or inactive.");

        if (dto.OrderAmount < coupon.MinOrderAmount)
            return Fail($"Minimum order amount is ₹{coupon.MinOrderAmount}.");

        if (dto.CustomerId.HasValue && coupon.MaxUsesPerCustomer.HasValue)
        {
            var uses = await _repo.GetUsageCountAsync(coupon.Id, dto.CustomerId.Value, ct);
            if (uses >= coupon.MaxUsesPerCustomer.Value)
                return Fail("You have already used this coupon the maximum number of times.");
        }

        var discount = coupon.CalculateDiscount(dto.OrderAmount);

        return new CouponValidationResultDto
        {
            IsValid = true,
            Message = "Coupon applied successfully.",
            DiscountAmount = discount,
            CouponId = coupon.Id,
            Code = coupon.Code
        };
    }

    private static CouponValidationResultDto Fail(string message) => new()
    {
        IsValid = false,
        Message = message,
        DiscountAmount = 0m
    };
}
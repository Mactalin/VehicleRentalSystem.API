using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.DTOs;
using VehicleRentalSystem.Application.ServiceInterfaces;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.RepositoryInterfaces;
using static Azure.Core.HttpHeader;

namespace VehicleRentalSystem.Application.Services;

public class CouponService : ICouponService
{
    private readonly ICouponRepository _repository;
    private readonly IMapper _mapper;

    public CouponService(ICouponRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CouponDto>> GetAllAsync()
        => _mapper.Map<IEnumerable<CouponDto>>(await _repository.GetAllAsync());

    public async Task<CouponDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<CouponDto>(entity);
    }

    public async Task<CouponDto?> GetByCodeAsync(string code)
    {
        var entity = await _repository.GetByCodeAsync(code);
        return entity == null ? null : _mapper.Map<CouponDto>(entity);
    }

    public async Task<IEnumerable<CouponDto>> GetActiveCouponsAsync()
        => _mapper.Map<IEnumerable<CouponDto>>(await _repository.GetActiveCouponsAsync());

    public async Task<CouponDto> CreateAsync(CreateCouponDto dto)
    {
        var entity = _mapper.Map<Coupon>(dto);
        entity.IsActive = true;
        entity.UsedCount = 0;
        await _repository.AddAsync(entity);
        return _mapper.Map<CouponDto>(entity);
    }

    public async Task<CouponDto> UpdateAsync(int id, CreateCouponDto dto)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Coupon not found");
        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity);
        return _mapper.Map<CouponDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Coupon not found");
        await _repository.DeleteAsync(entity);
    }

    public async Task<CouponValidationResultDto> ValidateAsync(ValidateCouponDto dto)
    {
        var coupon = await _repository.GetByCodeAsync(dto.Code);
        if (coupon == null || !coupon.IsActive)
            return new CouponValidationResultDto { IsValid = false, Message = "Invalid or inactive coupon." };

        var now = DateTime.UtcNow;
        if (now < coupon.ValidFrom || now > coupon.ValidTo)
            return new CouponValidationResultDto { IsValid = false, Message = "Coupon expired or not yet valid." };

        if (coupon.MaxUses.HasValue && coupon.UsedCount >= coupon.MaxUses.Value)
            return new CouponValidationResultDto { IsValid = false, Message = "Coupon usage limit reached." };

        decimal discount = coupon.DiscountType == DiscountType.Percentage
            ? dto.OrderAmount * (coupon.DiscountValue / 100m)
            : coupon.DiscountValue;

        if (discount > dto.OrderAmount) discount = dto.OrderAmount;

        return new CouponValidationResultDto
        {
            IsValid = true,
            DiscountAmount = discount,
            Message = "Coupon applied successfully."
        };
    }
}
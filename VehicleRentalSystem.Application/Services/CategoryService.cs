using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Category;
using VehicleRentalSystem.Application.Exceptions;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categories;
        private readonly IVehicleRepository _vehicles;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categories, IVehicleRepository vehicles, IMapper mapper)
        {
            _categories = categories;
            _vehicles = vehicles;
            _mapper = mapper;
        }

        // ---------- Categories ----------

        public async Task<IEnumerable<CategoryDto>> GetAllAsync() =>
            _mapper.Map<IEnumerable<CategoryDto>>(await _categories.GetAllWithPricingAsync());

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var category = await _categories.GetByIdWithPricingAsync(id)
                           ?? throw new NotFoundException("Category not found.");
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateAsync(SaveCategoryDto dto)
        {
            var name = dto.Name.Trim();
            if (await _categories.NameExistsAsync(name))
                throw new ConflictException($"Category '{name}' already exists.");

            var category = _mapper.Map<VehicleCategory>(dto);
            category.Name = name;

            await _categories.AddAsync(category);
            await _categories.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> UpdateAsync(int id, SaveCategoryDto dto)
        {
            var category = await _categories.GetByIdAsync(id) ?? throw new NotFoundException("Category not found.");

            var name = dto.Name.Trim();
            if (!string.Equals(name, category.Name, StringComparison.OrdinalIgnoreCase)
                && await _categories.NameExistsAsync(name))
                throw new ConflictException($"Category '{name}' already exists.");

            _mapper.Map(dto, category);
            category.Name = name;
            await _categories.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categories.GetByIdAsync(id) ?? throw new NotFoundException("Category not found.");

            if (await _vehicles.ExistsAsync(v => v.CategoryId == id && v.IsActive))
                throw new BadRequestException("This category still has active vehicles.");

            category.IsActive = false;   // soft delete
            await _categories.SaveChangesAsync();
        }

        // ---------- Base pricing ----------

        public async Task<PricingDto> GetPricingAsync(int categoryId)
        {
            var pricing = await _categories.GetPricingByCategoryIdAsync(categoryId)
                          ?? throw new NotFoundException("Pricing is not configured for this category.");
            return _mapper.Map<PricingDto>(pricing);
        }

        public async Task<PricingDto> UpsertPricingAsync(int categoryId, PricingDto dto)
        {
            _ = await _categories.GetByIdAsync(categoryId) ?? throw new NotFoundException("Category not found.");

            var pricing = await _categories.GetPricingByCategoryIdAsync(categoryId);
            if (pricing is null)
            {
                pricing = _mapper.Map<CategoryPricing>(dto);
                pricing.CategoryId = categoryId;
                await _categories.AddPricingAsync(pricing);
            }
            else
            {
                _mapper.Map(dto, pricing);
            }

            await _categories.SaveChangesAsync();
            return _mapper.Map<PricingDto>(pricing);
        }

        // ---------- Seasonal pricing ----------

        public async Task<IEnumerable<SeasonalPricingDto>> GetSeasonalPricingsAsync(int categoryId)
        {
            _ = await _categories.GetByIdAsync(categoryId) ?? throw new NotFoundException("Category not found.");
            return _mapper.Map<IEnumerable<SeasonalPricingDto>>(await _categories.GetSeasonalPricingsAsync(categoryId));
        }

        public async Task<SeasonalPricingDto> AddSeasonalPricingAsync(int categoryId, SaveSeasonalPricingDto dto)
        {
            _ = await _categories.GetByIdAsync(categoryId) ?? throw new NotFoundException("Category not found.");
            ValidateSeasonDates(dto);

            var season = _mapper.Map<SeasonalPricing>(dto);
            season.CategoryId = categoryId;

            await _categories.AddSeasonalPricingAsync(season);
            await _categories.SaveChangesAsync();
            return _mapper.Map<SeasonalPricingDto>(season);
        }

        public async Task<SeasonalPricingDto> UpdateSeasonalPricingAsync(int categoryId, int id, SaveSeasonalPricingDto dto)
        {
            var season = await GetSeasonAsync(categoryId, id);
            ValidateSeasonDates(dto);

            _mapper.Map(dto, season);
            await _categories.SaveChangesAsync();
            return _mapper.Map<SeasonalPricingDto>(season);
        }

        public async Task DeleteSeasonalPricingAsync(int categoryId, int id)
        {
            var season = await GetSeasonAsync(categoryId, id);
            _categories.DeleteSeasonalPricing(season);
            await _categories.SaveChangesAsync();
        }

        // ---------- Additional charges ----------

        public async Task<IEnumerable<AdditionalChargeDto>> GetAdditionalChargesAsync() =>
            _mapper.Map<IEnumerable<AdditionalChargeDto>>(await _categories.GetAdditionalChargesAsync());

        public async Task<AdditionalChargeDto> AddAdditionalChargeAsync(SaveAdditionalChargeDto dto)
        {
            var charge = _mapper.Map<AdditionalCharge>(dto);
            await _categories.AddAdditionalChargeAsync(charge);
            await _categories.SaveChangesAsync();
            return _mapper.Map<AdditionalChargeDto>(charge);
        }

        public async Task<AdditionalChargeDto> UpdateAdditionalChargeAsync(int id, SaveAdditionalChargeDto dto)
        {
            var charge = await _categories.GetAdditionalChargeByIdAsync(id)
                         ?? throw new NotFoundException("Additional charge not found.");

            _mapper.Map(dto, charge);
            await _categories.SaveChangesAsync();
            return _mapper.Map<AdditionalChargeDto>(charge);
        }

        public async Task DeleteAdditionalChargeAsync(int id)
        {
            var charge = await _categories.GetAdditionalChargeByIdAsync(id)
                         ?? throw new NotFoundException("Additional charge not found.");

            _categories.DeleteAdditionalCharge(charge);
            await _categories.SaveChangesAsync();
        }

        // ---------- Helpers ----------

        private static void ValidateSeasonDates(SaveSeasonalPricingDto dto)
        {
            if (dto.EndDate < dto.StartDate)
                throw new BadRequestException("End date cannot be before the start date.");
        }

        private async Task<SeasonalPricing> GetSeasonAsync(int categoryId, int id)
        {
            var season = await _categories.GetSeasonalPricingByIdAsync(id);
            if (season is null || season.CategoryId != categoryId)
                throw new NotFoundException("Seasonal pricing not found.");
            return season;
        }
    }
}

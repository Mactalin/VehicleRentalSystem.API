using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Category;

namespace VehicleRentalSystem.Application.Interface_Service
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(SaveCategoryDto dto);
        Task<CategoryDto> UpdateAsync(int id, SaveCategoryDto dto);
        Task DeleteAsync(int id);

        Task<PricingDto> GetPricingAsync(int categoryId);
        Task<PricingDto> UpsertPricingAsync(int categoryId, PricingDto dto);

        Task<IEnumerable<SeasonalPricingDto>> GetSeasonalPricingsAsync(int categoryId);
        Task<SeasonalPricingDto> AddSeasonalPricingAsync(int categoryId, SaveSeasonalPricingDto dto);
        Task<SeasonalPricingDto> UpdateSeasonalPricingAsync(int categoryId, int id, SaveSeasonalPricingDto dto);
        Task DeleteSeasonalPricingAsync(int categoryId, int id);

        Task<IEnumerable<AdditionalChargeDto>> GetAdditionalChargesAsync();
        Task<AdditionalChargeDto> AddAdditionalChargeAsync(SaveAdditionalChargeDto dto);
        Task<AdditionalChargeDto> UpdateAdditionalChargeAsync(int id, SaveAdditionalChargeDto dto);
        Task DeleteAdditionalChargeAsync(int id);
    }
}

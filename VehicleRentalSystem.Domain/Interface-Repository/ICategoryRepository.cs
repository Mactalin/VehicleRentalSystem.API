using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.Interface_Repository
{
    public interface ICategoryRepository : IGenericRepository<VehicleCategory>
    {
        Task<VehicleCategory?> GetByIdWithPricingAsync(int id);
        Task<IEnumerable<VehicleCategory>> GetAllWithPricingAsync();
        Task<bool> NameExistsAsync(string name);

        Task<CategoryPricing?> GetPricingByCategoryIdAsync(int categoryId);
        Task AddPricingAsync(CategoryPricing pricing);

        Task<IEnumerable<SeasonalPricing>> GetSeasonalPricingsAsync(int categoryId);
        Task<SeasonalPricing?> GetSeasonalPricingByIdAsync(int id);
        Task<SeasonalPricing?> GetActiveSeasonalPricingAsync(int categoryId, DateOnly date);
        Task AddSeasonalPricingAsync(SeasonalPricing seasonalPricing);
        void DeleteSeasonalPricing(SeasonalPricing seasonalPricing);

        Task<IEnumerable<AdditionalCharge>> GetAdditionalChargesAsync();
        Task<AdditionalCharge?> GetAdditionalChargeByIdAsync(int id);
        Task AddAdditionalChargeAsync(AdditionalCharge charge);
        void DeleteAdditionalCharge(AdditionalCharge charge);
    }
}

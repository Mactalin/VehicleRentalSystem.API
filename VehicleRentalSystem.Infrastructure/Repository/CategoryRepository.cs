using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository
{
    public class CategoryRepository : GenericRepository<VehicleCategory>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<VehicleCategory?> GetByIdWithPricingAsync(int id) =>
            await _dbSet.Include(c => c.Pricing)
                        .Include(c => c.SeasonalPricings)
                        .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<VehicleCategory>> GetAllWithPricingAsync() =>
            await _dbSet.AsNoTracking().Include(c => c.Pricing).OrderBy(c => c.Name).ToListAsync();

        public async Task<bool> NameExistsAsync(string name) =>
            await _dbSet.AnyAsync(c => c.Name == name);

        // Base pricing
        public async Task<CategoryPricing?> GetPricingByCategoryIdAsync(int categoryId) =>
            await _context.CategoryPricings.FirstOrDefaultAsync(p => p.CategoryId == categoryId);

        public async Task AddPricingAsync(CategoryPricing pricing) =>
            await _context.CategoryPricings.AddAsync(pricing);

        // Seasonal pricing
        public async Task<IEnumerable<SeasonalPricing>> GetSeasonalPricingsAsync(int categoryId) =>
            await _context.SeasonalPricings.AsNoTracking()
                .Where(s => s.CategoryId == categoryId)
                .OrderBy(s => s.StartDate).ToListAsync();

        public async Task<SeasonalPricing?> GetSeasonalPricingByIdAsync(int id) =>
            await _context.SeasonalPricings.FindAsync(id);

        public async Task<SeasonalPricing?> GetActiveSeasonalPricingAsync(int categoryId, DateOnly date) =>
            await _context.SeasonalPricings.AsNoTracking()
                .Where(s => s.CategoryId == categoryId && s.IsActive
                            && s.StartDate <= date && s.EndDate >= date)
                .OrderByDescending(s => s.Multiplier)
                .FirstOrDefaultAsync();

        public async Task AddSeasonalPricingAsync(SeasonalPricing seasonalPricing) =>
            await _context.SeasonalPricings.AddAsync(seasonalPricing);

        public void DeleteSeasonalPricing(SeasonalPricing seasonalPricing) =>
            _context.SeasonalPricings.Remove(seasonalPricing);

        // Additional charges
        public async Task<IEnumerable<AdditionalCharge>> GetAdditionalChargesAsync() =>
            await _context.AdditionalCharges.AsNoTracking().OrderBy(a => a.Name).ToListAsync();

        public async Task<AdditionalCharge?> GetAdditionalChargeByIdAsync(int id) =>
            await _context.AdditionalCharges.FindAsync(id);

        public async Task AddAdditionalChargeAsync(AdditionalCharge charge) =>
            await _context.AdditionalCharges.AddAsync(charge);

        public void DeleteAdditionalCharge(AdditionalCharge charge) =>
            _context.AdditionalCharges.Remove(charge);
    }
}

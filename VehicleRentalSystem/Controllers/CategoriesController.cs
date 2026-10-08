using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.Dtos.Category;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Authorization;

namespace VehicleRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categories;

        public CategoriesController(ICategoryService categories) => _categories = categories;

        // ----- Categories -----

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll() =>
            Ok(await _categories.GetAllAsync());

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryDto>> GetById(int id) =>
            Ok(await _categories.GetByIdAsync(id));

        [HttpPost]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<ActionResult<CategoryDto>> Create(SaveCategoryDto dto)
        {
            var created = await _categories.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<ActionResult<CategoryDto>> Update(int id, SaveCategoryDto dto) =>
            Ok(await _categories.UpdateAsync(id, dto));

        [HttpDelete("{id:int}")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<IActionResult> Delete(int id)
        {
            await _categories.DeleteAsync(id);
            return NoContent();
        }

        // ----- Base pricing (hourly / daily / weekly / deposit) -----

        [HttpGet("{id:int}/pricing")]
        [AllowAnonymous]
        public async Task<ActionResult<PricingDto>> GetPricing(int id) =>
            Ok(await _categories.GetPricingAsync(id));

        [HttpPut("{id:int}/pricing")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<ActionResult<PricingDto>> UpsertPricing(int id, PricingDto dto) =>
            Ok(await _categories.UpsertPricingAsync(id, dto));

        // ----- Seasonal pricing -----

        [HttpGet("{id:int}/seasonal-pricing")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SeasonalPricingDto>>> GetSeasonalPricings(int id) =>
            Ok(await _categories.GetSeasonalPricingsAsync(id));

        [HttpPost("{id:int}/seasonal-pricing")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<ActionResult<SeasonalPricingDto>> AddSeasonalPricing(int id, SaveSeasonalPricingDto dto) =>
            Ok(await _categories.AddSeasonalPricingAsync(id, dto));

        [HttpPut("{id:int}/seasonal-pricing/{seasonId:int}")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<ActionResult<SeasonalPricingDto>> UpdateSeasonalPricing(
            int id, int seasonId, SaveSeasonalPricingDto dto) =>
            Ok(await _categories.UpdateSeasonalPricingAsync(id, seasonId, dto));

        [HttpDelete("{id:int}/seasonal-pricing/{seasonId:int}")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<IActionResult> DeleteSeasonalPricing(int id, int seasonId)
        {
            await _categories.DeleteSeasonalPricingAsync(id, seasonId);
            return NoContent();
        }

        // ----- Additional charges (late fee, extra km, cleaning, ...) -----

        [HttpGet("additional-charges")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AdditionalChargeDto>>> GetAdditionalCharges() =>
            Ok(await _categories.GetAdditionalChargesAsync());

        [HttpPost("additional-charges")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<ActionResult<AdditionalChargeDto>> AddAdditionalCharge(SaveAdditionalChargeDto dto) =>
            Ok(await _categories.AddAdditionalChargeAsync(dto));

        [HttpPut("additional-charges/{chargeId:int}")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<ActionResult<AdditionalChargeDto>> UpdateAdditionalCharge(
            int chargeId, SaveAdditionalChargeDto dto) =>
            Ok(await _categories.UpdateAdditionalChargeAsync(chargeId, dto));

        [HttpDelete("additional-charges/{chargeId:int}")]
        [Authorize(Policy = Policies.AdminOrManager)]
        public async Task<IActionResult> DeleteAdditionalCharge(int chargeId)
        {
            await _categories.DeleteAdditionalChargeAsync(chargeId);
            return NoContent();
        }
    }
}

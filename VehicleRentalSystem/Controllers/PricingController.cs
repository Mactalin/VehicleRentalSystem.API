using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Application.Dtos.Category;
using VehicleRentalSystem.Application.Interface_Service;

namespace VehicleRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PricingController : ControllerBase
    {
        private readonly IPricingService _pricing;

        public PricingController(IPricingService pricing) => _pricing = pricing;

        // GET api/pricing/quote?vehicleId=1&from=2026-10-10T09:00&to=2026-10-12T18:00
        [HttpGet("quote")]
        public async Task<ActionResult<PriceQuoteDto>> GetQuote(
            [FromQuery] int vehicleId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to) =>
            Ok(await _pricing.GetQuoteAsync(vehicleId, from, to));
    }
}

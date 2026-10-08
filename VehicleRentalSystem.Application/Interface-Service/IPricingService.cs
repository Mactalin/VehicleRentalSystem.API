using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Category;

namespace VehicleRentalSystem.Application.Interface_Service
{
    public interface IPricingService
    {
        Task<PriceQuoteDto> GetQuoteAsync(int vehicleId, DateTime from, DateTime to);
    }
}

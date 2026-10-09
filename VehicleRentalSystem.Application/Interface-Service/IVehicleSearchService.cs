using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Customers;
using VehicleRentalSystem.Application.Dtos.Search;

namespace VehicleRentalSystem.Application.Interface_Service
{

    public interface IVehicleSearchService
    {
        Task<PagedResult<VehicleSearchResultDto>> SearchAsync(VehicleSearchRequestDto request, CancellationToken ct = default);
        Task<AvailabilityDto> CheckAvailabilityAsync(int vehicleId, DateTime from, DateTime to, CancellationToken ct = default);
    }
}

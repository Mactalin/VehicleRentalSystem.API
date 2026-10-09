using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Customers;
using VehicleRentalSystem.Application.Dtos.Search;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Exceptions;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services
{
    public class VehicleSearchService : IVehicleSearchService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IPriceCalculator _pricing;

        public VehicleSearchService(IUnitOfWork uow, IMapper mapper, IPriceCalculator pricing)
        {
            _uow = uow;
            _mapper = mapper;
            _pricing = pricing;
        }

        public async Task<PagedResult<VehicleSearchResultDto>> SearchAsync(
            VehicleSearchRequestDto request, CancellationToken ct = default)
        {
            if (request.From.HasValue != request.To.HasValue)
                throw new DomainException("Provide both 'from' and 'to', or neither.");

            var hasDates = request.From.HasValue && request.To.HasValue;
            if (hasDates)
            {
                if (request.From < DateTime.UtcNow.AddMinutes(-5))
                    throw new DomainException("The pickup time cannot be in the past.");
                if (request.To <= request.From)
                    throw new DomainException("The return time must be after the pickup time.");
            }

            if (request.MinPrice > request.MaxPrice)
                throw new DomainException("The minimum price cannot be greater than the maximum price.");

            var criteria = new VehicleSearchCriteria
            {
                BranchId = request.BranchId,
                City = request.City,
                From = request.From,
                To = request.To,
                CategoryId = request.CategoryId,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                FeatureIds = request.FeatureIds,
                Page = request.Page,
                PageSize = request.PageSize
            };

            var (vehicles, total) = await _uow.Vehicles.SearchAvailableAsync(criteria, ct);
            var results = _mapper.Map<List<VehicleSearchResultDto>>(vehicles);

            // Attach an estimated price when the customer searched with dates
            if (hasDates)
            {
                for (var i = 0; i < vehicles.Count; i++)
                    results[i].Quote = _pricing.Calculate(vehicles[i], request.From!.Value, request.To!.Value);
            }

            return new PagedResult<VehicleSearchResultDto>
            {
                Items = results,
                Page = Math.Max(1, request.Page),
                PageSize = Math.Clamp(request.PageSize, 1, 100),
                TotalCount = total
            };
        }

        public async Task<AvailabilityDto> CheckAvailabilityAsync(
            int vehicleId, DateTime from, DateTime to, CancellationToken ct = default)
        {
            if (to <= from)
                throw new DomainException("The return time must be after the pickup time.");

            var vehicle = await _uow.Vehicles.GetByIdAsync(vehicleId, ct)
                ?? throw new NotFoundException(nameof(Vehicle), vehicleId);

            var free = vehicle.IsActive && await _uow.Bookings.IsVehicleAvailableAsync(vehicleId, from, to, null, ct);

            return new AvailabilityDto { VehicleId = vehicleId, From = from, To = to, IsAvailable = free };
        }
    }
}

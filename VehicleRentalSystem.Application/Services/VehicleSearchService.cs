using AutoMapper;
using VehicleRentalSystem.Application.Common;
using VehicleRentalSystem.Application.Dtos.Customers;
using VehicleRentalSystem.Application.Dtos.Search;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Exceptions;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services;

public class VehicleSearchService : IVehicleSearchService
{
    private readonly IUnitOfWork _uow;
    private readonly IVehicleSearchRepository _search;
    private readonly IPricingRepository _pricingRepo;
    private readonly IPriceCalculator _pricing;
    private readonly IMapper _mapper;
    private readonly BranchClock _clock;

    public VehicleSearchService(
        IUnitOfWork uow, IVehicleSearchRepository search, IPricingRepository pricingRepo,
        IPriceCalculator pricing, IMapper mapper, BranchClock clock)
    {
        _uow = uow;
        _search = search;
        _pricingRepo = pricingRepo;
        _pricing = pricing;
        _mapper = mapper;
        _clock = clock;
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
            FuelType = request.FuelType,
            Transmission = request.Transmission,
            MinSeats = request.MinSeatingCapacity,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var (vehicles, total) = await _search.SearchAvailableAsync(criteria, ct);
        var results = _mapper.Map<List<VehicleSearchResultDto>>(vehicles);

        if (hasDates)
        {
            var from = request.From!.Value;
            var to = request.To!.Value;
            var pickupDate = _clock.LocalDate(from);
            var multipliers = new Dictionary<int, decimal>();      // one lookup per category

            for (var i = 0; i < vehicles.Count; i++)
            {
                var pricing = vehicles[i].Category.Pricing;
                if (pricing is null) continue;

                var categoryId = vehicles[i].CategoryId;
                if (!multipliers.TryGetValue(categoryId, out var multiplier))
                    multipliers[categoryId] = multiplier =
                        await _pricingRepo.GetSeasonalMultiplierAsync(categoryId, pickupDate, ct);

                results[i].SeasonalMultiplier = multiplier;
                results[i].Quote = _pricing.Calculate(pricing, from, to, seasonalMultiplier: multiplier);
            }
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

        var free = vehicle.IsActive
            && await _uow.Bookings.IsVehicleAvailableAsync(vehicleId, from, to, null, ct);

        return new AvailabilityDto { VehicleId = vehicleId, From = from, To = to, IsAvailable = free };
    }
}
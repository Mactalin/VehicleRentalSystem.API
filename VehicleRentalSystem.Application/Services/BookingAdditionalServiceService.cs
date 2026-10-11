using VehicleRentalSystem.Application.DTOs.BookingServices;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Exceptions;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services;

public class BookingAdditionalServiceService : IBookingAdditionalServiceService
{
    private readonly IBookingAdditionalServiceRepository _repo;
    private readonly IAdditionalServiceRepository _serviceRepo;

    public BookingAdditionalServiceService(
        IBookingAdditionalServiceRepository repo,
        IAdditionalServiceRepository serviceRepo)
    {
        _repo = repo;
        _serviceRepo = serviceRepo;
    }

    public async Task<IEnumerable<BookingAdditionalServiceDto>> GetByBookingIdAsync(int bookingId, CancellationToken ct = default)
    {
        var links = await _repo.GetByBookingIdAsync(bookingId, ct);
        return links.Select(Map);
    }

    public async Task<BookingAdditionalServiceDto> AddAsync(AddServiceToBookingDto dto, CancellationToken ct = default)
    {
        var service = await _serviceRepo.GetByIdAsync(dto.AdditionalServiceId, ct)
            ?? throw new NotFoundException(nameof(AdditionalService), dto.AdditionalServiceId);

        if (!service.IsActive)
            throw new DomainException($"Additional service '{service.Name}' is not available.");

        if (service.MaxQuantityPerBooking.HasValue && dto.Quantity > service.MaxQuantityPerBooking.Value)
            throw new DomainException($"Quantity exceeds maximum allowed ({service.MaxQuantityPerBooking}) for '{service.Name}'.");

        var link = new BookingAdditionalService
        {
            BookingId = dto.BookingId,
            AdditionalServiceId = dto.AdditionalServiceId,
            Quantity = dto.Quantity,
            UnitPrice = service.Price,
            LineTotal = service.Price * dto.Quantity
        };

        await _repo.AddAsync(link, ct);
        await _repo.SaveChangesAsync(ct);

        link.AdditionalService = service;
        return Map(link);
    }

    public async Task RemoveAsync(int bookingId, int additionalServiceId, CancellationToken ct = default)
    {
        var links = await _repo.GetByBookingIdAsync(bookingId, ct);
        var link = links.FirstOrDefault(l => l.AdditionalServiceId == additionalServiceId)
            ?? throw new NotFoundException(nameof(BookingAdditionalService), $"{bookingId}/{additionalServiceId}");

        _repo.Delete(link);
        await _repo.SaveChangesAsync(ct);
    }

    private static BookingAdditionalServiceDto Map(BookingAdditionalService link) => new()
    {
        Id = link.Id,
        BookingId = link.BookingId,
        AdditionalServiceId = link.AdditionalServiceId,
        ServiceName = link.AdditionalService?.Name ?? string.Empty,
        Quantity = link.Quantity,
        UnitPrice = link.UnitPrice,
        LineTotal = link.LineTotal
    };
}
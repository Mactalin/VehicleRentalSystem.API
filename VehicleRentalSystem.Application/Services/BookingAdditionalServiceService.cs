using VehicleRentalSystem.Application.DTOs;
using VehicleRentalSystem.Application.ServiceInterfaces;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.RepositoryInterfaces;

namespace VehicleRentalSystem.Application.Services;

public class BookingAdditionalServiceService : IBookingAdditionalServiceService
{
    private readonly IBookingAdditionalServiceRepository _linkRepo;
    private readonly IAdditionalServiceRepository _serviceRepo;

    public BookingAdditionalServiceService(
        IBookingAdditionalServiceRepository linkRepo,
        IAdditionalServiceRepository serviceRepo)
    {
        _linkRepo = linkRepo;
        _serviceRepo = serviceRepo;
    }

    public async Task<IEnumerable<BookingAdditionalServiceDto>> GetByBookingIdAsync(int bookingId)
    {
        var links = await _linkRepo.GetByBookingIdAsync(bookingId);
        return links.Select(bas => new BookingAdditionalServiceDto
        {
            BookingId = bas.BookingId,
            AdditionalServiceId = bas.AdditionalServiceId,
            ServiceName = bas.AdditionalService.Name,
            Price = bas.Price
        });
    }

    public async Task<BookingAdditionalServiceDto> AddServiceAsync(AddServiceToBookingDto dto)
    {
        var service = await _serviceRepo.GetByIdAsync(dto.AdditionalServiceId)
            ?? throw new KeyNotFoundException("Additional service not found");

        var link = new BookingAdditionalService
        {
            BookingId = dto.BookingId,
            AdditionalServiceId = dto.AdditionalServiceId,
            Price = service.Price
        };

        await _linkRepo.AddAsync(link);
        await _linkRepo.SaveChangesAsync();

        return new BookingAdditionalServiceDto
        {
            BookingId = link.BookingId,
            AdditionalServiceId = link.AdditionalServiceId,
            ServiceName = service.Name,
            Price = link.Price
        };
    }

    public async Task RemoveServiceAsync(int bookingId, int additionalServiceId)
    {
        var link = await _linkRepo.GetAsync(bookingId, additionalServiceId)
            ?? throw new KeyNotFoundException("Link not found");

        await _linkRepo.RemoveAsync(link);
        await _linkRepo.SaveChangesAsync();
    }
}
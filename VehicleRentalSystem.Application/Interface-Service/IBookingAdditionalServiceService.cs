using VehicleRentalSystem.Application.DTOs.BookingServices;

namespace VehicleRentalSystem.Application.Interface_Service;

public interface IBookingAdditionalServiceService
{
    Task<IEnumerable<BookingAdditionalServiceDto>> GetByBookingIdAsync(int bookingId, CancellationToken ct = default);
    Task<BookingAdditionalServiceDto> AddAsync(AddServiceToBookingDto dto, CancellationToken ct = default);
    Task RemoveAsync(int bookingId, int additionalServiceId, CancellationToken ct = default);
}
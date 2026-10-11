using VehicleRentalSystem.Application.DTOs.BookingDrivers;

namespace VehicleRentalSystem.Application.Interface_Service;

public interface IBookingDriverService
{
    Task<IEnumerable<BookingDriverDto>> GetByBookingIdAsync(int bookingId, CancellationToken ct = default);
    Task<IEnumerable<BookingDriverDto>> GetByDriverIdAsync(int driverId, CancellationToken ct = default);
    Task<BookingDriverDto> AssignAsync(AssignDriverDto dto, CancellationToken ct = default);
    Task UnassignAsync(int bookingId, int driverId, CancellationToken ct = default);
}
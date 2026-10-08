using VehicleRentalSystem.Application.DTOs;
using VehicleRentalSystem.Application.ServiceInterfaces;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.RepositoryInterfaces;

namespace VehicleRentalSystem.Application.Services;

public class BookingDriverService : IBookingDriverService
{
    private readonly IBookingDriverRepository _bookingDriverRepo;
    private readonly IDriverRepository _driverRepo;

    public BookingDriverService(
        IBookingDriverRepository bookingDriverRepo,
        IDriverRepository driverRepo)
    {
        _bookingDriverRepo = bookingDriverRepo;
        _driverRepo = driverRepo;
    }

    public async Task<IEnumerable<BookingDriverDto>> GetByBookingIdAsync(int bookingId)
    {
        var links = await _bookingDriverRepo.GetByBookingIdAsync(bookingId);
        return links.Select(bd => new BookingDriverDto
        {
            BookingId = bd.BookingId,
            DriverId = bd.DriverId,
            DriverName = bd.Driver.FullName,
            DriverCharge = bd.DriverCharge
        });
    }

    public async Task<BookingDriverDto> AssignDriverAsync(AssignDriverDto dto)
    {
        var driver = await _driverRepo.GetByIdAsync(dto.DriverId)
            ?? throw new KeyNotFoundException("Driver not found");

        var link = new BookingDriver
        {
            BookingId = dto.BookingId,
            DriverId = dto.DriverId,
            DriverCharge = dto.DriverCharge
        };

        driver.IsAvailable = false;
        await _driverRepo.UpdateAsync(driver);

        await _bookingDriverRepo.AddAsync(link);
        await _bookingDriverRepo.SaveChangesAsync();

        return new BookingDriverDto
        {
            BookingId = link.BookingId,
            DriverId = link.DriverId,
            DriverName = driver.FullName,
            DriverCharge = link.DriverCharge
        };
    }

    public async Task UnassignDriverAsync(int bookingId, int driverId)
    {
        var link = await _bookingDriverRepo.GetAsync(bookingId, driverId)
            ?? throw new KeyNotFoundException("Assignment not found");

        var driver = await _driverRepo.GetByIdAsync(driverId);
        if (driver != null)
        {
            driver.IsAvailable = true;
            await _driverRepo.UpdateAsync(driver);
        }

        await _bookingDriverRepo.RemoveAsync(link);
        await _bookingDriverRepo.SaveChangesAsync();
    }
}
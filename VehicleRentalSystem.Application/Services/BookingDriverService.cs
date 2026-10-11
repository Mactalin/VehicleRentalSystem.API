using VehicleRentalSystem.Application.DTOs.BookingDrivers;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Exceptions;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services;

public class BookingDriverService : IBookingDriverService
{
    private readonly IBookingDriverRepository _repo;
    private readonly IDriverRepository _driverRepo;

    public BookingDriverService(IBookingDriverRepository repo, IDriverRepository driverRepo)
    {
        _repo = repo;
        _driverRepo = driverRepo;
    }

    public async Task<IEnumerable<BookingDriverDto>> GetByBookingIdAsync(int bookingId, CancellationToken ct = default)
    {
        var links = await _repo.GetByBookingIdAsync(bookingId, ct);
        return links.Select(Map);
    }

    public async Task<IEnumerable<BookingDriverDto>> GetByDriverIdAsync(int driverId, CancellationToken ct = default)
    {
        var links = await _repo.GetByDriverIdAsync(driverId, ct);
        return links.Select(Map);
    }

    public async Task<BookingDriverDto> AssignAsync(AssignDriverDto dto, CancellationToken ct = default)
    {
        var driver = await _driverRepo.GetByIdAsync(dto.DriverId, ct)
            ?? throw new NotFoundException(nameof(Driver), dto.DriverId);

        if (!driver.IsAvailable())
            throw new DomainException($"Driver '{driver.FullName}' is not available.");

        var alreadyAssigned = await _repo.GetActiveAssignmentAsync(dto.BookingId, dto.DriverId, ct);
        if (alreadyAssigned is not null)
            throw new DomainException("This driver is already assigned to this booking.");

        var link = new BookingDriver
        {
            BookingId = dto.BookingId,
            DriverId = dto.DriverId,
            ChargeApplied = dto.ChargeApplied,
            AssignedAt = DateTime.UtcNow,
            IsActive = true
        };

        driver.Status = Domain.Enums.DriverStatus.OnDuty;
        _driverRepo.Update(driver);

        await _repo.AddAsync(link, ct);
        await _repo.SaveChangesAsync(ct);

        link.Driver = driver;
        return Map(link);
    }

    public async Task UnassignAsync(int bookingId, int driverId, CancellationToken ct = default)
    {
        var link = await _repo.GetActiveAssignmentAsync(bookingId, driverId, ct)
            ?? throw new NotFoundException(nameof(BookingDriver), $"{bookingId}/{driverId}");

        link.IsActive = false;
        link.ReleasedAt = DateTime.UtcNow;
        _repo.Update(link);

        var driver = await _driverRepo.GetByIdAsync(driverId, ct);
        if (driver is not null)
        {
            driver.Status = Domain.Enums.DriverStatus.Available;
            _driverRepo.Update(driver);
        }

        await _repo.SaveChangesAsync(ct);
    }

    private static BookingDriverDto Map(BookingDriver bd) => new()
    {
        Id = bd.Id,
        BookingId = bd.BookingId,
        DriverId = bd.DriverId,
        DriverName = bd.Driver?.FullName ?? string.Empty,
        DriverPhone = bd.Driver?.Phone ?? string.Empty,
        ChargeApplied = bd.ChargeApplied,
        AssignedAt = bd.AssignedAt,
        ReleasedAt = bd.ReleasedAt,
        IsActive = bd.IsActive
    };
}
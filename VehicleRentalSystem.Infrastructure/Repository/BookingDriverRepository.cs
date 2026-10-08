using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.RepositoryInterfaces;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repositories;

public class BookingDriverRepository : IBookingDriverRepository
{
    private readonly ApplicationDbContext _context;

    public BookingDriverRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<BookingDriver>> GetByBookingIdAsync(int bookingId)
    {
        return await _context.BookingDrivers
            .Include(bd => bd.Driver)
            .Where(bd => bd.BookingId == bookingId)
            .ToListAsync();
    }

    public async Task<BookingDriver?> GetAsync(int bookingId, int driverId)
    {
        return await _context.BookingDrivers
            .Include(bd => bd.Driver)
            .FirstOrDefaultAsync(bd => bd.BookingId == bookingId && bd.DriverId == driverId);
    }

    public async Task AddAsync(BookingDriver link) => await _context.BookingDrivers.AddAsync(link);

    public Task RemoveAsync(BookingDriver link)
    {
        _context.BookingDrivers.Remove(link);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}

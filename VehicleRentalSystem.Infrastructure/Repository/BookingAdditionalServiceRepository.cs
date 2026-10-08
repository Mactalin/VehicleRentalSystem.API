using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.RepositoryInterfaces;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repositories;

public class BookingAdditionalServiceRepository : IBookingAdditionalServiceRepository
{
    private readonly ApplicationDbContext _context;

    public BookingAdditionalServiceRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<BookingAdditionalService>> GetByBookingIdAsync(int bookingId)
    {
        return await _context.BookingAdditionalServices
            .Include(bas => bas.AdditionalService)
            .Where(bas => bas.BookingId == bookingId)
            .ToListAsync();
    }

    public async Task<BookingAdditionalService?> GetAsync(int bookingId, int additionalServiceId)
    {
        return await _context.BookingAdditionalServices
            .Include(bas => bas.AdditionalService)
            .FirstOrDefaultAsync(bas => bas.BookingId == bookingId
                                     && bas.AdditionalServiceId == additionalServiceId);
    }

    public async Task AddAsync(BookingAdditionalService link)
        => await _context.BookingAdditionalServices.AddAsync(link);

    public Task RemoveAsync(BookingAdditionalService link)
    {
        _context.BookingAdditionalServices.Remove(link);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}

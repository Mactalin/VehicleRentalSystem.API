using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.RepositoryInterfaces;

public interface IBookingDriverRepository
{
    Task<IEnumerable<BookingDriver>> GetByBookingIdAsync(int bookingId);
    Task<BookingDriver?> GetAsync(int bookingId, int driverId);
    Task AddAsync(BookingDriver link);
    Task RemoveAsync(BookingDriver link);
    Task SaveChangesAsync();
}

using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Domain.RepositoryInterfaces;

public interface IBookingAdditionalServiceRepository
{
    Task<IEnumerable<BookingAdditionalService>> GetByBookingIdAsync(int bookingId);
    Task<BookingAdditionalService?> GetAsync(int bookingId, int additionalServiceId);
    Task AddAsync(BookingAdditionalService link);
    Task RemoveAsync(BookingAdditionalService link);
    Task SaveChangesAsync();
}
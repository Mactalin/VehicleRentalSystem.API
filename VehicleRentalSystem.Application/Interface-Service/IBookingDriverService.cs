using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.DTOs;

namespace VehicleRentalSystem.Application.ServiceInterfaces;

public interface IBookingDriverService
{
    Task<IEnumerable<BookingDriverDto>> GetByBookingIdAsync(int bookingId);
    Task<BookingDriverDto> AssignDriverAsync(AssignDriverDto dto);
    Task UnassignDriverAsync(int bookingId, int driverId);
}
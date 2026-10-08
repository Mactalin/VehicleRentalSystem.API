using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.DTOs;

namespace VehicleRentalSystem.Application.ServiceInterfaces;

public interface IBookingAdditionalServiceService
{
    Task<IEnumerable<BookingAdditionalServiceDto>> GetByBookingIdAsync(int bookingId);
    Task<BookingAdditionalServiceDto> AddServiceAsync(AddServiceToBookingDto dto);
    Task RemoveServiceAsync(int bookingId, int additionalServiceId);
}
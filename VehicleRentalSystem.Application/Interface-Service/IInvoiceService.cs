using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Payments;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Application.Interface_Service
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> GetByBookingAsync(int bookingId, CancellationToken ct = default);

        /// Builds and adds the invoice to the unit of work. Does NOT call SaveChanges.
        Task<Invoice> CreateForBookingAsync(Booking booking, CancellationToken ct = default);
    }
}

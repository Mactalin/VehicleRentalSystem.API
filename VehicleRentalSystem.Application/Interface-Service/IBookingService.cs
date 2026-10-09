using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Bookings;

namespace VehicleRentalSystem.Application.Interface_Service
{
    public interface IBookingService
    {
        Task<PriceQuoteDto> QuoteAsync(PriceQuoteRequestDto dto, CancellationToken ct = default);
        Task<BookingDto> CreateAsync(CreateBookingDto dto, CancellationToken ct = default);
        Task<BookingDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<BookingSummaryDto>> GetByCustomerAsync(int customerId, CancellationToken ct = default);
        Task<BookingDto> ModifyAsync(int id, ModifyBookingDto dto, int actingUserId, CancellationToken ct = default);
        Task<CancellationResultDto> CancelAsync(int id, CancelBookingDto dto, CancellationToken ct = default);
        Task<BookingDto> PickupAsync(int id, PickupDto dto, int staffUserId, CancellationToken ct = default);
        Task<ReturnSummaryDto> ReturnAsync(int id, ReturnDto dto, int staffUserId, CancellationToken ct = default);
        Task<BookingDto> CompleteAsync(int id, CancellationToken ct = default);

        /// Expires unpaid Pending bookings. Call from a background worker every minute or so.
        Task<int> ExpireStaleBookingsAsync(CancellationToken ct = default);
    }
}

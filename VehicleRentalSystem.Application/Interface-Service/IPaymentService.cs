using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Payments;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Application.Interface_Service
{
    public interface IPaymentService
    {
        Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto, CancellationToken ct = default);
        Task<RefundDto> RefundAsync(RefundRequestDto dto, CancellationToken ct = default);
        Task<IReadOnlyList<PaymentDto>> GetByBookingAsync(int bookingId, CancellationToken ct = default);
        Task<IReadOnlyList<PaymentDto>> GetByCustomerAsync(int customerId, CancellationToken ct = default);
        Task<SettlementDto> GetSettlementAsync(int bookingId, CancellationToken ct = default);

        // Used by BookingService inside its own transaction. These do NOT call SaveChanges.
        Task<decimal> RefundOnCancellationAsync(Booking booking, DateTime now, CancellationToken ct = default);
        Task<SettlementDto> SettleAsync(Booking booking, CancellationToken ct = default);
    }
}

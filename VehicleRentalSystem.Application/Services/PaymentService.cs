using AutoMapper;
using VehicleRentalSystem.Application.Common;
using VehicleRentalSystem.Application.Dtos.Payments;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;

using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Exceptions;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IPaymentGateway _gateway;
    private readonly BillingSettings _settings;

    public PaymentService(IUnitOfWork uow, IMapper mapper, IPaymentGateway gateway, BillingSettings settings)
    {
        _uow = uow;
        _mapper = mapper;
        _gateway = gateway;
        _settings = settings;
    }

    // ---------------- Take a payment ----------------

    public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto, CancellationToken ct = default)
    {
        var booking = await _uow.Bookings.GetByIdAsync(dto.BookingId, ct)
            ?? throw new NotFoundException(nameof(Booking), dto.BookingId);

        var now = DateTime.UtcNow;

        if (booking.Status == BookingStatus.Pending && booking.PendingExpiresAt <= now)
            throw new DomainException("This booking has expired. Please create a new booking.");

        var payments = await _uow.Payments.GetByBookingAsync(booking.Id, ct);
        var settlement = BuildSettlement(booking, payments);

        // The server decides the amount. The client never sends it.
        decimal amount;
        switch (dto.Type)
        {
            case PaymentType.Advance:
                RequireStatus(booking, BookingStatus.Pending);
                if (settlement.RentalPaid > 0)
                    throw new DomainException("The advance has already been paid.");
                amount = booking.AdvanceAmount;
                break;

            case PaymentType.Full:
                RequireStatus(booking, BookingStatus.Pending, BookingStatus.Confirmed,
                    BookingStatus.PickedUp, BookingStatus.Returned);
                amount = settlement.RentalBalance;
                break;

            case PaymentType.SecurityDeposit:
                RequireStatus(booking, BookingStatus.Confirmed);
                if (settlement.DepositPaid > 0)
                    throw new DomainException("The security deposit has already been paid.");
                amount = booking.SecurityDeposit;
                break;

            case PaymentType.LateFee:
                RequireStatus(booking, BookingStatus.Returned);
                amount = settlement.LateFeeDue;
                break;

            case PaymentType.DamageCharge:
                RequireStatus(booking, BookingStatus.Returned);
                amount = settlement.DamageChargeDue;
                break;

            default:
                throw new DomainException("Unsupported payment type.");
        }

        if (amount <= 0)
            throw new DomainException("There is nothing to pay for this payment type.");

        var result = await _gateway.ChargeAsync(
            amount, "INR", dto.Method, $"{dto.Type} for {booking.BookingNumber}", ct);

        var payment = new Payment
        {
            BookingId = booking.Id,
            Type = dto.Type,
            Method = dto.Method,
            Amount = amount,
            Currency = "INR",
            Status = result.Success ? PaymentStatus.Success : PaymentStatus.Failed,
            GatewayReference = result.Reference,
            FailureReason = result.FailureReason,
            PaidAt = result.Success ? now : null
        };

        await _uow.Payments.AddAsync(payment, ct);

        // Paying the advance (or everything) confirms a Pending booking
        if (result.Success && booking.Status == BookingStatus.Pending
            && dto.Type is PaymentType.Advance or PaymentType.Full)
        {
            booking.Confirm();
        }

        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<PaymentDto>(payment);
    }

    // ---------------- Manual refund (admin) ----------------

    public async Task<RefundDto> RefundAsync(RefundRequestDto dto, CancellationToken ct = default)
    {
        var found = await _uow.Payments.GetByIdAsync(dto.PaymentId, ct)
            ?? throw new NotFoundException(nameof(Payment), dto.PaymentId);

        // Reload with refunds so RefundableAmount is correct
        var payment = (await _uow.Payments.GetByBookingAsync(found.BookingId, ct))
            .First(p => p.Id == dto.PaymentId);

        if (payment.Status != PaymentStatus.Success)
            throw new DomainException("Only successful payments can be refunded.");

        if (dto.Amount > payment.RefundableAmount)
            throw new DomainException($"The maximum refundable amount is {payment.RefundableAmount:0.00}.");

        var refund = await CreateRefundAsync(payment, dto.Amount, dto.Reason, ct);
        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<RefundDto>(refund);
    }

    // ---------------- Read ----------------

    public async Task<IReadOnlyList<PaymentDto>> GetByBookingAsync(int bookingId, CancellationToken ct = default)
    {
        var payments = await _uow.Payments.GetByBookingAsync(bookingId, ct);
        return _mapper.Map<List<PaymentDto>>(payments);
    }

    public async Task<IReadOnlyList<PaymentDto>> GetByCustomerAsync(int customerId, CancellationToken ct = default)
    {
        var payments = await _uow.Payments.GetByCustomerAsync(customerId, ct);
        return _mapper.Map<List<PaymentDto>>(payments);
    }

    public async Task<SettlementDto> GetSettlementAsync(int bookingId, CancellationToken ct = default)
    {
        var booking = await _uow.Bookings.GetByIdAsync(bookingId, ct)
            ?? throw new NotFoundException(nameof(Booking), bookingId);

        var payments = await _uow.Payments.GetByBookingAsync(bookingId, ct);
        return BuildSettlement(booking, payments);
    }

    // ---------------- Used by BookingService (no SaveChanges here) ----------------

    /// Rental payments (advance/full) are refunded by policy. The security deposit is always refunded in full.
    public async Task<decimal> RefundOnCancellationAsync(Booking booking, DateTime now, CancellationToken ct = default)
    {
        var payments = await _uow.Payments.GetByBookingAsync(booking.Id, ct);

        var percent = (booking.StartAt - now).TotalHours > _settings.FullRefundBeforeHours
            ? 1m
            : _settings.PartialRefundPercent;

        decimal total = 0m;

        foreach (var payment in payments.Where(p => p.Status == PaymentStatus.Success))
        {
            var share = payment.Type switch
            {
                PaymentType.Advance or PaymentType.Full => percent,
                PaymentType.SecurityDeposit => 1m,
                _ => 0m
            };

            var amount = Math.Round(payment.RefundableAmount * share, 2);
            if (amount <= 0) continue;

            await CreateRefundAsync(payment, amount, "Booking cancellation", ct);
            total += amount;
        }

        return total;
    }

    /// Checks nothing is owed, then returns whatever is left of the deposit.
    public async Task<SettlementDto> SettleAsync(Booking booking, CancellationToken ct = default)
    {
        var payments = await _uow.Payments.GetByBookingAsync(booking.Id, ct);
        var settlement = BuildSettlement(booking, payments);

        if (settlement.TotalDue > 0)
            throw new DomainException(
                $"An outstanding amount of {settlement.TotalDue:0.00} must be paid before the rental can be completed.");

        if (settlement.DepositToRefund > 0)
        {
            var depositPayment = payments.FirstOrDefault(p =>
                p.Type == PaymentType.SecurityDeposit && p.RefundableAmount >= settlement.DepositToRefund)
                ?? throw new DomainException("Unable to find the deposit payment to refund.");

            await CreateRefundAsync(depositPayment, settlement.DepositToRefund, "Security deposit return", ct);
        }

        settlement.IsSettled = true;
        return settlement;
    }

    // ---------------- Helpers ----------------

    private async Task<Refund> CreateRefundAsync(Payment payment, decimal amount, string reason, CancellationToken ct)
    {
        // Cash was handed over at the counter, so there is no gateway call to make
        var result = payment.Method == PaymentMethod.Cash || payment.GatewayReference is null
            ? new GatewayResult(true, null, null)
            : await _gateway.RefundAsync(payment.GatewayReference, amount, ct);

        if (!result.Success)
            throw new DomainException($"The refund failed: {result.FailureReason}");

        var refund = new Refund
        {
            PaymentId = payment.Id,
            BookingId = payment.BookingId,
            Amount = amount,
            Reason = reason,
            Status = RefundStatus.Processed,
            GatewayReference = result.Reference,
            ProcessedAt = DateTime.UtcNow
        };

        payment.Refunds.Add(refund);

        if (payment.RefundableAmount <= 0)
            payment.Status = PaymentStatus.Refunded;

        return refund;
    }

    private static void RequireStatus(Booking booking, params BookingStatus[] allowed)
    {
        if (!allowed.Contains(booking.Status))
            throw new DomainException($"This payment is not allowed while the booking is {booking.Status}.");
    }

    /// Late fee is taken from the deposit first, then damage. Whatever the deposit cannot cover is "due".
    private static SettlementDto BuildSettlement(Booking booking, IReadOnlyList<Payment> payments)
    {
        static bool IsPaid(Payment p) => p.Status is PaymentStatus.Success or PaymentStatus.Refunded;

        decimal Sum(PaymentType type) =>
            payments.Where(p => p.Type == type && IsPaid(p)).Sum(p => p.Amount);

        var rentalPaid = Sum(PaymentType.Advance) + Sum(PaymentType.Full);
        var rentalBalance = Math.Max(0m, booking.TotalAmount - rentalPaid);

        var depositPaid = Sum(PaymentType.SecurityDeposit);
        var depositRefunded = payments
            .Where(p => p.Type == PaymentType.SecurityDeposit)
            .Sum(p => p.RefundedAmount);

        var lateFromDeposit = Math.Min(depositPaid, booking.LateFee);
        var damageFromDeposit = Math.Min(depositPaid - lateFromDeposit, booking.DamageCharge);

        var lateDue = Math.Max(0m, booking.LateFee - lateFromDeposit - Sum(PaymentType.LateFee));
        var damageDue = Math.Max(0m, booking.DamageCharge - damageFromDeposit - Sum(PaymentType.DamageCharge));

        var depositToRefund = Math.Max(0m,
            depositPaid - lateFromDeposit - damageFromDeposit - depositRefunded);

        var totalDue = rentalBalance + lateDue + damageDue;

        return new SettlementDto
        {
            BookingId = booking.Id,
            TotalAmount = booking.TotalAmount,
            RentalPaid = rentalPaid,
            RentalBalance = rentalBalance,
            DepositPaid = depositPaid,
            DepositRefunded = depositRefunded,
            LateFee = booking.LateFee,
            DamageCharge = booking.DamageCharge,
            LateFeeDue = lateDue,
            DamageChargeDue = damageDue,
            DepositToRefund = depositToRefund,
            TotalDue = totalDue,
            IsSettled = totalDue == 0m
                && booking.Status is BookingStatus.Returned or BookingStatus.Completed
        };
    }
}
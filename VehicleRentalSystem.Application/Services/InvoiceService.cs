using AutoMapper;
using VehicleRentalSystem.Application.Dtos.Payments;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;

using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Exceptions;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public InvoiceService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<InvoiceDto> GetByBookingAsync(int bookingId, CancellationToken ct = default)
    {
        var invoice = await _uow.Invoices.GetByBookingAsync(bookingId, ct)
            ?? throw new NotFoundException("Invoice for booking", bookingId);

        var dto = _mapper.Map<InvoiceDto>(invoice);

        var customer = await _uow.Customers.GetByIdAsync(invoice.CustomerId, ct);
        var booking = await _uow.Bookings.GetByIdAsync(invoice.BookingId, ct);

        dto.CustomerName = customer?.FullName ?? string.Empty;
        dto.BookingNumber = booking?.BookingNumber ?? string.Empty;
        return dto;
    }

    /// The booking's price snapshot becomes the invoice lines. The deposit is not invoiced
    /// because it is refundable and not a charge. Late and damage charges carry no GST here.
    public async Task<Invoice> CreateForBookingAsync(Booking booking, CancellationToken ct = default)
    {
        var subtotal = booking.BaseAmount + booking.AddOnAmount - booking.DiscountAmount;

        // Reuse the GST rate that was applied when the booking was priced
        var taxRate = subtotal > 0 ? Math.Round(booking.TaxAmount / subtotal, 4) : 0m;

        var invoice = new Invoice
        {
            InvoiceNumber = await _uow.Invoices.GenerateInvoiceNumberAsync(ct),
            BookingId = booking.Id,
            CustomerId = booking.CustomerId,
            IssuedAt = DateTime.UtcNow,
            Status = InvoiceStatus.Paid
        };

        var hasRate = booking.RateApplied > 0;
        var units = hasRate ? Math.Max(1, (int)Math.Round(booking.BaseAmount / booking.RateApplied)) : 1;
        var unitPrice = hasRate ? booking.RateApplied : booking.BaseAmount;
        var unitName = booking.RateType == RateType.Hourly ? "hour(s)" : "day(s)";

        invoice.Items.Add(new InvoiceItem
        {
            Description = $"Vehicle rental: {booking.Vehicle?.Name} ({units} {unitName})",
            Quantity = units,
            UnitPrice = unitPrice,
            TaxRate = taxRate
        });

        if (booking.AddOnAmount > 0)
            invoice.Items.Add(new InvoiceItem
            {
                Description = "Add-ons and accessories",
                UnitPrice = booking.AddOnAmount,
                TaxRate = taxRate
            });

        if (booking.DiscountAmount > 0)
            invoice.Items.Add(new InvoiceItem
            {
                Description = string.IsNullOrWhiteSpace(booking.CouponCode)
                    ? "Discount"
                    : $"Discount ({booking.CouponCode})",
                UnitPrice = -booking.DiscountAmount,
                TaxRate = taxRate
            });

        if (booking.LateFee > 0)
            invoice.Items.Add(new InvoiceItem
            {
                Description = "Late return charges",
                UnitPrice = booking.LateFee,
                TaxRate = 0m
            });

        if (booking.DamageCharge > 0)
            invoice.Items.Add(new InvoiceItem
            {
                Description = "Damage charges",
                UnitPrice = booking.DamageCharge,
                TaxRate = 0m
            });

        await _uow.Invoices.AddAsync(invoice, ct);
        return invoice;
    }
}
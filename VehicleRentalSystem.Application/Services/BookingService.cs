using AutoMapper;
using VehicleRentalSystem.Application.Common;
using VehicleRentalSystem.Application.Dtos.Bookings;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Exceptions;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IPriceCalculator _pricing;
    private readonly IPaymentService _payments;
    private readonly IInvoiceService _invoices;
    private readonly IPricingRepository _pricingRepo;
    private readonly IBranchLookupRepository _branches;
    private readonly BranchClock _clock;
    private readonly BillingSettings _settings;

    public BookingService(
        IUnitOfWork uow, IMapper mapper, IPriceCalculator pricing,
        IPaymentService payments, IInvoiceService invoices,
        IPricingRepository pricingRepo, IBranchLookupRepository branches,
        BranchClock clock, BillingSettings settings)
    {
        _uow = uow;
        _mapper = mapper;
        _pricing = pricing;
        _payments = payments;
        _invoices = invoices;
        _pricingRepo = pricingRepo;
        _branches = branches;
        _clock = clock;
        _settings = settings;
    }

    // ---------------- Quote and create ----------------

    public async Task<PriceQuoteDto> QuoteAsync(PriceQuoteRequestDto dto, CancellationToken ct = default)
    {
        ValidateWindow(dto.StartAt, dto.EndAt, DateTime.UtcNow);

        var vehicle = await _uow.Vehicles.GetByIdAsync(dto.VehicleId, ct)
            ?? throw new NotFoundException(nameof(Vehicle), dto.VehicleId);

        return await PriceAsync(vehicle.CategoryId, dto.StartAt, dto.EndAt, 0m, 0m, ct);
    }

    public async Task<BookingDto> CreateAsync(CreateBookingDto dto, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        ValidateWindow(dto.StartAt, dto.EndAt, now);

        var customer = await _uow.Customers.GetWithDocumentsAsync(dto.CustomerId, ct)
            ?? throw new NotFoundException(nameof(Customer), dto.CustomerId);

        // Active account, verified KYC, and a licence valid through the RETURN date
        if (!customer.CanBook(dto.EndAt, out var reason))
            throw new DomainException(reason);

        var vehicle = await _uow.Vehicles.GetByIdAsync(dto.VehicleId, ct)
            ?? throw new NotFoundException(nameof(Vehicle), dto.VehicleId);

        if (!vehicle.IsActive || vehicle.Status == VehicleStatus.Maintenance)
            throw new DomainException("This vehicle is not available for rental.");

        // The vehicle is physically at its own branch, so pickup must be there
        if (dto.PickupBranchId.HasValue && dto.PickupBranchId.Value != vehicle.BranchId)
            throw new DomainException("This vehicle is located at a different branch.");

        var pickupBranch = vehicle.BranchId;
        var dropBranch = dto.DropBranchId ?? pickupBranch;

        if (!await _branches.BranchExistsAsync(dropBranch, ct))
            throw new NotFoundException("Branch", dropBranch);

        await EnsureBranchOpenAsync(pickupBranch, dto.StartAt, "pickup", ct);
        await EnsureBranchOpenAsync(dropBranch, dto.EndAt, "return", ct);

        // TODO (Lenin): add-on charges and coupon discount go in here
        var quote = await PriceAsync(vehicle.CategoryId, dto.StartAt, dto.EndAt, 0m, 0m, ct);

        await _uow.BeginTransactionAsync(ct);
        try
        {
            // Re-check inside the transaction (bookings AND maintenance) so two customers cannot grab the same slot
            if (!await _uow.Bookings.IsVehicleAvailableAsync(vehicle.Id, dto.StartAt, dto.EndAt, null, ct))
                throw new BookingConflictException();

            var booking = new Booking
            {
                BookingNumber = $"BK-{now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",
                CustomerId = customer.Id,
                VehicleId = vehicle.Id,
                PickupBranchId = pickupBranch,
                DropBranchId = dropBranch,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
                Status = BookingStatus.Pending,
                PendingExpiresAt = now.AddMinutes(_settings.PendingExpiryMinutes),

                // Price snapshot
                RateType = quote.RateType,
                RateApplied = quote.RateApplied,
                BaseAmount = quote.BaseAmount,
                AddOnAmount = quote.AddOnAmount,
                DiscountAmount = quote.DiscountAmount,
                TaxAmount = quote.TaxAmount,
                TotalAmount = quote.TotalAmount,
                SecurityDeposit = quote.SecurityDeposit,
                AdvanceAmount = quote.AdvanceAmount,
                LateGraceHours = _settings.LateGraceHours,
                LatePenaltyMultiplier = _settings.LatePenaltyMultiplier
            };

            await _uow.Bookings.AddAsync(booking, ct);
            await _uow.CommitAsync(ct);

            return await GetByIdAsync(booking.Id, ct);
        }
        catch
        {
            await _uow.RollbackAsync(ct);
            throw;
        }
    }

    // ---------------- Read ----------------

    public async Task<BookingDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var booking = await LoadAsync(id, ct);
        return _mapper.Map<BookingDto>(booking);
    }

    public async Task<IReadOnlyList<BookingSummaryDto>> GetByCustomerAsync(int customerId, CancellationToken ct = default)
    {
        var bookings = await _uow.Bookings.GetByCustomerAsync(customerId, ct);
        return _mapper.Map<List<BookingSummaryDto>>(bookings);
    }

    // ---------------- Modify ----------------

    public async Task<BookingDto> ModifyAsync(int id, ModifyBookingDto dto, int actingUserId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var booking = await LoadAsync(id, ct);

        if (booking.Status is not (BookingStatus.Pending or BookingStatus.Confirmed))
            throw new DomainException($"A booking in '{booking.Status}' state cannot be modified.");

        ValidateWindow(dto.NewStartAt, dto.NewEndAt, now);

        var customer = await _uow.Customers.GetWithDocumentsAsync(booking.CustomerId, ct)
            ?? throw new NotFoundException(nameof(Customer), booking.CustomerId);

        if (!customer.CanBook(dto.NewEndAt, out var reason))
            throw new DomainException(reason);

        await EnsureBranchOpenAsync(booking.PickupBranchId, dto.NewStartAt, "pickup", ct);
        await EnsureBranchOpenAsync(booking.DropBranchId, dto.NewEndAt, "return", ct);

        // The new dates are re-priced at current rates
        var quote = await PriceAsync(booking.Vehicle.CategoryId, dto.NewStartAt, dto.NewEndAt,
            booking.AddOnAmount, booking.DiscountAmount, ct);

        await _uow.BeginTransactionAsync(ct);
        try
        {
            if (!await _uow.Bookings.IsVehicleAvailableAsync(
                    booking.VehicleId, dto.NewStartAt, dto.NewEndAt, booking.Id, ct))
                throw new BookingConflictException();

            booking.Modifications.Add(new BookingModification
            {
                OldStartAt = booking.StartAt,
                OldEndAt = booking.EndAt,
                NewStartAt = dto.NewStartAt,
                NewEndAt = dto.NewEndAt,
                PriceDifference = quote.TotalAmount - booking.TotalAmount,
                Reason = dto.Reason,
                ModifiedByUserId = actingUserId
            });

            booking.StartAt = dto.NewStartAt;
            booking.EndAt = dto.NewEndAt;
            booking.RateType = quote.RateType;
            booking.RateApplied = quote.RateApplied;
            booking.BaseAmount = quote.BaseAmount;
            booking.TaxAmount = quote.TaxAmount;
            booking.TotalAmount = quote.TotalAmount;

            // A paid advance stays as it is. An unpaid one follows the new total.
            if (booking.Status == BookingStatus.Pending)
                booking.AdvanceAmount = quote.AdvanceAmount;

            await _uow.CommitAsync(ct);
            return await GetByIdAsync(booking.Id, ct);
        }
        catch
        {
            await _uow.RollbackAsync(ct);
            throw;
        }
    }

    // ---------------- Cancel ----------------

    public async Task<CancellationResultDto> CancelAsync(int id, CancelBookingDto dto, CancellationToken ct = default)
    {
        var booking = await LoadAsync(id, ct);
        var now = DateTime.UtcNow;

        await _uow.BeginTransactionAsync(ct);
        try
        {
            booking.Cancel(dto.Reason, now);                       // throws if already picked up etc.
            var refunded = await _payments.RefundOnCancellationAsync(booking, now, ct);
            await _uow.CommitAsync(ct);

            return new CancellationResultDto
            {
                BookingId = booking.Id,
                Status = booking.Status,
                RefundAmount = refunded
            };
        }
        catch
        {
            await _uow.RollbackAsync(ct);
            throw;
        }
    }

    // ---------------- Pickup ----------------

    public async Task<BookingDto> PickupAsync(int id, PickupDto dto, int staffUserId, CancellationToken ct = default)
    {
        var booking = await LoadAsync(id, ct);
        var now = DateTime.UtcNow;

        if (booking.Status != BookingStatus.Confirmed)
            throw new DomainException($"Only a Confirmed booking can be picked up. Current status: {booking.Status}.");

        if (now < booking.StartAt.AddMinutes(-_settings.PickupEarlyMinutes))
            throw new DomainException("It is too early to pick up this vehicle.");

        if (now > booking.EndAt)
            throw new DomainException("The booking period has already ended.");

        // Licence could have expired or been rejected since booking
        var customer = await _uow.Customers.GetWithDocumentsAsync(booking.CustomerId, ct)
            ?? throw new NotFoundException(nameof(Customer), booking.CustomerId);

        if (!customer.CanBook(booking.EndAt, out var reason))
            throw new DomainException(reason);

        var depositPaid = booking.Payments.Any(p =>
            p.Type == PaymentType.SecurityDeposit && p.Status == PaymentStatus.Success);

        if (booking.SecurityDeposit > 0 && !depositPaid)
            throw new DomainException("The security deposit must be paid before pickup.");

        booking.Inspections.Add(new VehicleInspection
        {
            Type = InspectionType.Pickup,
            OdometerReading = dto.OdometerReading,
            FuelLevelPercent = dto.FuelLevelPercent,
            DamageNotes = dto.DamageNotes,
            PhotoUrls = dto.PhotoUrls,
            InspectedAt = now,
            InspectedByUserId = staffUserId
        });

        booking.Vehicle.Status = VehicleStatus.Rented;
        booking.MarkPickedUp(now);

        await _uow.SaveChangesAsync(ct);
        return await GetByIdAsync(booking.Id, ct);
    }

    // ---------------- Return ----------------

    public async Task<ReturnSummaryDto> ReturnAsync(int id, ReturnDto dto, int staffUserId, CancellationToken ct = default)
    {
        var booking = await LoadAsync(id, ct);
        var now = DateTime.UtcNow;

        if (booking.Status != BookingStatus.PickedUp)
            throw new DomainException($"Only a PickedUp booking can be returned. Current status: {booking.Status}.");

        var pickup = booking.Inspections.FirstOrDefault(i => i.Type == InspectionType.Pickup);
        if (pickup is not null && dto.OdometerReading < pickup.OdometerReading)
            throw new DomainException("The return odometer reading cannot be lower than the pickup reading.");

        booking.Inspections.Add(new VehicleInspection
        {
            Type = InspectionType.Return,
            OdometerReading = dto.OdometerReading,
            FuelLevelPercent = dto.FuelLevelPercent,
            DamageNotes = dto.DamageNotes,
            PhotoUrls = dto.PhotoUrls,
            DamageCharge = dto.DamageCharge,
            InspectedAt = now,
            InspectedByUserId = staffUserId
        });

        booking.MarkReturned(now, dto.DamageCharge);       // calculates the late fee

        // The vehicle is back, and it now sits at the drop-off branch
        booking.Vehicle.Status = VehicleStatus.Available;
        booking.Vehicle.OdometerKm = dto.OdometerReading;
        booking.Vehicle.BranchId = booking.DropBranchId;

        await _uow.SaveChangesAsync(ct);

        return new ReturnSummaryDto
        {
            BookingId = booking.Id,
            Status = booking.Status,
            ReturnedAt = now,
            LateFee = booking.LateFee,
            DamageCharge = booking.DamageCharge,
            Settlement = await _payments.GetSettlementAsync(booking.Id, ct)
        };
    }

    // ---------------- Complete ----------------

    public async Task<BookingDto> CompleteAsync(int id, CancellationToken ct = default)
    {
        var booking = await LoadAsync(id, ct);

        if (booking.Status != BookingStatus.Returned)
            throw new DomainException($"Only a Returned booking can be completed. Current status: {booking.Status}.");

        await _uow.BeginTransactionAsync(ct);
        try
        {
            await _payments.SettleAsync(booking, ct);          // throws if money is still owed, refunds leftover deposit
            await _invoices.CreateForBookingAsync(booking, ct);
            booking.Complete();

            await _uow.CommitAsync(ct);
            return await GetByIdAsync(booking.Id, ct);
        }
        catch
        {
            await _uow.RollbackAsync(ct);
            throw;
        }
    }

    // ---------------- Background expiry ----------------

    public async Task<int> ExpireStaleBookingsAsync(CancellationToken ct = default)
    {
        var stale = await _uow.Bookings.GetExpiredPendingAsync(DateTime.UtcNow, ct);
        foreach (var booking in stale)
            booking.Expire();

        if (stale.Count > 0)
            await _uow.SaveChangesAsync(ct);

        return stale.Count;
    }

    // ---------------- Helpers ----------------

    private async Task<Booking> LoadAsync(int id, CancellationToken ct) =>
        await _uow.Bookings.GetWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(nameof(Booking), id);

    private static void ValidateWindow(DateTime start, DateTime end, DateTime now)
    {
        if (start < now.AddMinutes(-5))
            throw new DomainException("The start time cannot be in the past.");
        if (end <= start)
            throw new DomainException("The end time must be after the start time.");
    }

    private async Task<PriceQuoteDto> PriceAsync(
        int categoryId, DateTime start, DateTime end, decimal addOn, decimal discount, CancellationToken ct)
    {
        var pricing = await _pricingRepo.GetPricingAsync(categoryId, ct)
            ?? throw new DomainException("No pricing is configured for this vehicle's category.");

        var multiplier = await _pricingRepo.GetSeasonalMultiplierAsync(categoryId, _clock.LocalDate(start), ct);
        return _pricing.Calculate(pricing, start, end, addOn, discount, multiplier);
    }

    /// Branch hours are in local time. A branch with no hours defined for that day is not restricted.
    private async Task EnsureBranchOpenAsync(int branchId, DateTime utcTime, string what, CancellationToken ct)
    {
        if (!_settings.EnforceBranchHours) return;

        var local = _clock.ToLocal(utcTime);
        var hours = await _branches.GetOperatingHourAsync(branchId, local.DayOfWeek, ct);
        if (hours is null) return;

        if (hours.IsClosed)
            throw new DomainException($"The branch is closed on {local:dddd}. Please choose another {what} day.");

        var time = TimeOnly.FromDateTime(local);
        if (time < hours.OpenTime || time > hours.CloseTime)
            throw new DomainException(
                $"The requested {what} time ({local:ddd HH:mm}) is outside branch hours " +
                $"({hours.OpenTime:HH\\:mm} to {hours.CloseTime:HH\\:mm}).");
    }
}
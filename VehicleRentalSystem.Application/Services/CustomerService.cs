using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Common;
using VehicleRentalSystem.Application.Dtos.Bookings;
using VehicleRentalSystem.Application.Dtos.Customers;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Enums;
using VehicleRentalSystem.Domain.Exceptions;
using VehicleRentalSystem.Domain.Interface_Repository;

namespace VehicleRentalSystem.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly BillingSettings _settings;

        public CustomerService(IUnitOfWork uow, IMapper mapper, BillingSettings settings)
        {
            _uow = uow;
            _mapper = mapper;
            _settings = settings;
        }

        public async Task<CustomerDto> RegisterAsync(CreateCustomerDto dto, CancellationToken ct = default)
        {
            if (await _uow.Customers.GetByUserIdAsync(dto.UserId, ct) is not null)
                throw new DomainException("A customer profile already exists for this user.");

            var customer = _mapper.Map<Customer>(dto);
            EnsureAdult(customer);

            await _uow.Customers.AddAsync(customer, ct);
            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var customer = await LoadWithDocsAsync(id, ct);
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            var customer = await _uow.Customers.GetByUserIdAsync(userId, ct)
                ?? throw new NotFoundException("Customer for user", userId);
            return await GetByIdAsync(customer.Id, ct);
        }

        public async Task<PagedResult<CustomerSummaryDto>> GetPagedAsync(CustomerFilterDto filter, CancellationToken ct = default)
        {
            var (items, total) = await _uow.Customers.GetPagedAsync(
                filter.Page, filter.PageSize, filter.Verification, filter.Status, filter.Search, ct);

            return new PagedResult<CustomerSummaryDto>
            {
                Items = _mapper.Map<List<CustomerSummaryDto>>(items),
                Page = Math.Max(1, filter.Page),
                PageSize = Math.Clamp(filter.PageSize, 1, 100),
                TotalCount = total
            };
        }

        public async Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto, CancellationToken ct = default)
        {
            var customer = await _uow.Customers.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Customer), id);

            _mapper.Map(dto, customer);
            EnsureAdult(customer);

            await _uow.SaveChangesAsync(ct);
            return await GetByIdAsync(id, ct);
        }

        public async Task<CustomerDto> SetStatusAsync(int id, UpdateCustomerStatusDto dto, CancellationToken ct = default)
        {
            var customer = await _uow.Customers.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Customer), id);

            customer.Status = dto.Status;
            await _uow.SaveChangesAsync(ct);
            return await GetByIdAsync(id, ct);
        }

        // ---------------- KYC and licence ----------------

        public async Task<KycDocumentDto> AddKycDocumentAsync(int customerId, KycUploadDto dto, CancellationToken ct = default)
        {
            var customer = await LoadWithDocsAsync(customerId, ct);

            var doc = _mapper.Map<KycDocument>(dto);
            doc.DocumentNumber = dto.DocumentNumber.Trim().ToUpperInvariant();
            doc.Status = VerificationStatus.Pending;

            customer.KycDocuments.Add(doc);
            RecalculateVerification(customer);

            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<KycDocumentDto>(doc);
        }

        public async Task<DrivingLicenceDto> AddLicenceAsync(int customerId, LicenceUploadDto dto, CancellationToken ct = default)
        {
            var customer = await LoadWithDocsAsync(customerId, ct);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (dto.ExpiryDate <= today)
                throw new DomainException("This driving licence has already expired.");
            if (dto.IssueDate >= dto.ExpiryDate)
                throw new DomainException("The licence issue date must be before the expiry date.");

            var number = dto.LicenceNumber.Trim().ToUpperInvariant();
            if (await _uow.Customers.LicenceNumberExistsAsync(number, ct))
                throw new DomainException("This licence number is already registered.");

            var licence = _mapper.Map<DrivingLicence>(dto);
            licence.LicenceNumber = number;
            licence.Status = VerificationStatus.Pending;

            customer.DrivingLicences.Add(licence);
            RecalculateVerification(customer);

            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<DrivingLicenceDto>(licence);
        }

        public async Task<CustomerDto> ReviewKycAsync(
            int customerId, int documentId, ReviewDocumentDto dto, int staffUserId, CancellationToken ct = default)
        {
            EnsureReasonWhenRejecting(dto);
            var customer = await LoadWithDocsAsync(customerId, ct);

            var doc = customer.KycDocuments.FirstOrDefault(d => d.Id == documentId)
                ?? throw new NotFoundException(nameof(KycDocument), documentId);

            doc.Status = dto.Approve ? VerificationStatus.Verified : VerificationStatus.Rejected;
            doc.RejectionReason = dto.Approve ? null : dto.RejectionReason;
            doc.VerifiedAt = DateTime.UtcNow;
            doc.VerifiedByUserId = staffUserId;

            RecalculateVerification(customer);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> ReviewLicenceAsync(
            int customerId, int licenceId, ReviewDocumentDto dto, int staffUserId, CancellationToken ct = default)
        {
            EnsureReasonWhenRejecting(dto);
            var customer = await LoadWithDocsAsync(customerId, ct);

            var licence = customer.DrivingLicences.FirstOrDefault(l => l.Id == licenceId)
                ?? throw new NotFoundException(nameof(DrivingLicence), licenceId);

            licence.Status = dto.Approve ? VerificationStatus.Verified : VerificationStatus.Rejected;
            licence.RejectionReason = dto.Approve ? null : dto.RejectionReason;
            licence.VerifiedAt = DateTime.UtcNow;
            licence.VerifiedByUserId = staffUserId;

            RecalculateVerification(customer);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<CustomerDto>(customer);
        }

        // ---------------- Rental history ----------------

        public async Task<IReadOnlyList<BookingSummaryDto>> GetRentalHistoryAsync(int customerId, CancellationToken ct = default)
        {
            _ = await _uow.Customers.GetByIdAsync(customerId, ct)
                ?? throw new NotFoundException(nameof(Customer), customerId);

            var bookings = await _uow.Bookings.GetByCustomerAsync(customerId, ct);
            return _mapper.Map<List<BookingSummaryDto>>(bookings);
        }

        // ---------------- Helpers ----------------

        private async Task<Customer> LoadWithDocsAsync(int id, CancellationToken ct) =>
            await _uow.Customers.GetWithDocumentsAsync(id, ct)
                ?? throw new NotFoundException(nameof(Customer), id);

        private void EnsureAdult(Customer customer)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (customer.Age(today) < _settings.MinimumCustomerAge)
                throw new DomainException($"Customers must be at least {_settings.MinimumCustomerAge} years old.");
        }

        private static void EnsureReasonWhenRejecting(ReviewDocumentDto dto)
        {
            if (!dto.Approve && string.IsNullOrWhiteSpace(dto.RejectionReason))
                throw new DomainException("A rejection reason is required.");
        }

        /// Verified = at least one verified KYC document AND one verified licence.
        /// Rejected = a required kind has nothing verified, nothing pending, and something rejected.
        private static void RecalculateVerification(Customer c)
        {
            var kycOk = c.KycDocuments.Any(d => d.Status == VerificationStatus.Verified);
            var licOk = c.DrivingLicences.Any(l => l.Status == VerificationStatus.Verified);

            if (kycOk && licOk)
            {
                c.VerificationStatus = VerificationStatus.Verified;
                return;
            }

            var kycRejected = !kycOk
                && !c.KycDocuments.Any(d => d.Status == VerificationStatus.Pending)
                && c.KycDocuments.Any(d => d.Status == VerificationStatus.Rejected);

            var licRejected = !licOk
                && !c.DrivingLicences.Any(l => l.Status == VerificationStatus.Pending)
                && c.DrivingLicences.Any(l => l.Status == VerificationStatus.Rejected);

            c.VerificationStatus = (kycRejected || licRejected)
                ? VerificationStatus.Rejected
                : VerificationStatus.Pending;
        }
    }
}

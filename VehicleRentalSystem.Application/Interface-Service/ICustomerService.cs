using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Dtos.Bookings;
using VehicleRentalSystem.Application.Dtos.Customers;

namespace VehicleRentalSystem.Application.Interface_Service
{
    public interface ICustomerService
    {
        Task<CustomerDto> RegisterAsync(CreateCustomerDto dto, CancellationToken ct = default);
        Task<CustomerDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<CustomerDto> GetByUserIdAsync(int userId, CancellationToken ct = default);
        Task<PagedResult<CustomerSummaryDto>> GetPagedAsync(CustomerFilterDto filter, CancellationToken ct = default);
        Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto, CancellationToken ct = default);
        Task<CustomerDto> SetStatusAsync(int id, UpdateCustomerStatusDto dto, CancellationToken ct = default);

        Task<KycDocumentDto> AddKycDocumentAsync(int customerId, KycUploadDto dto, CancellationToken ct = default);
        Task<DrivingLicenceDto> AddLicenceAsync(int customerId, LicenceUploadDto dto, CancellationToken ct = default);
        Task<CustomerDto> ReviewKycAsync(int customerId, int documentId, ReviewDocumentDto dto, int staffUserId, CancellationToken ct = default);
        Task<CustomerDto> ReviewLicenceAsync(int customerId, int licenceId, ReviewDocumentDto dto, int staffUserId, CancellationToken ct = default);

        Task<IReadOnlyList<BookingSummaryDto>> GetRentalHistoryAsync(int customerId, CancellationToken ct = default);
    }
}

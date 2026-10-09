using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Customers
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public CustomerStatus Status { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public List<KycDocumentDto> KycDocuments { get; set; } = new();
        public List<DrivingLicenceDto> DrivingLicences { get; set; } = new();
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Customers
{
    public class DrivingLicenceDto
    {
        public int Id { get; set; }
        public string LicenceNumber { get; set; } = string.Empty;
        public string IssuingAuthority { get; set; } = string.Empty;
        public string VehicleClasses { get; set; } = string.Empty;
        public DateOnly IssueDate { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public VerificationStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? VerifiedAt { get; set; }
    }
}

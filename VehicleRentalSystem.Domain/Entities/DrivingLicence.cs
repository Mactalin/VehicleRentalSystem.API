

using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Entities;

public class DrivingLicence : BaseEntity
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public string LicenceNumber { get; set; } = string.Empty;
    public string IssuingAuthority { get; set; } = string.Empty;
    public string VehicleClasses { get; set; } = string.Empty;   // e.g. "LMV,MCWG"
    public DateOnly IssueDate { get; set; }
    public DateOnly ExpiryDate { get; set; }
    public string FilePath { get; set; } = string.Empty;

    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
    public string? RejectionReason { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public int? VerifiedByUserId { get; set; }

    public bool IsValidOn(DateOnly date) =>
        Status == VerificationStatus.Verified && IssueDate <= date && ExpiryDate >= date;
}
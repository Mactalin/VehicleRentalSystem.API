

using VehicleRentalSystem.Domain.Common;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Entities;

public class Customer : BaseEntity
{
    public int UserId { get; set; }              // FK to Anbu's User table

    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;

    public CustomerStatus Status { get; set; } = CustomerStatus.Active;
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;

    public ICollection<KycDocument> KycDocuments { get; set; } = new List<KycDocument>();
    public ICollection<DrivingLicence> DrivingLicences { get; set; } = new List<DrivingLicence>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public int Age(DateOnly today) =>
        today.Year - DateOfBirth.Year - (today < DateOfBirth.AddYears(today.Year - DateOfBirth.Year) ? 1 : 0);

    /// Customer must be active, verified, and hold a licence valid through the return date.
    public bool CanBook(DateTime returnDate, out string reason)
    {
        if (Status != CustomerStatus.Active) { reason = $"Account is {Status}."; return false; }
        if (VerificationStatus != VerificationStatus.Verified) { reason = "KYC is not verified."; return false; }

        var returnOn = DateOnly.FromDateTime(returnDate);
        var hasValidLicence = DrivingLicences.Any(l => l.IsValidOn(returnOn));
        if (!hasValidLicence) { reason = "No approved driving licence valid through the return date."; return false; }

        reason = string.Empty;
        return true;
    }
}
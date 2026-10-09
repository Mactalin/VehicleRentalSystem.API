namespace VehicleRentalSystem.Domain.Entities;

// PLACEHOLDER: Anbu owns Vehicle and Branch. Delete this file once his entities are merged,
// and keep the property names he uses (Id, CategoryId, BranchId, DailyRate, ...) in sync with the search query.
public class Vehicle : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int BranchId { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal SecurityDeposit { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Branch : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}
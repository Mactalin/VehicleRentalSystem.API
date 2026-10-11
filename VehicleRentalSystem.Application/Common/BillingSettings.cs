namespace VehicleRentalSystem.Application.Common;

public class BillingSettings
{
    public decimal GstRate { get; set; } = 0.18m;
    public decimal AdvancePercent { get; set; } = 0.30m;
    public int PendingExpiryMinutes { get; set; } = 15;

    public decimal LateGraceHours { get; set; } = 1m;
    public decimal LatePenaltyMultiplier { get; set; } = 1.5m;

    public int FullRefundBeforeHours { get; set; } = 24;
    public decimal PartialRefundPercent { get; set; } = 0.50m;

    public int MinimumRentalHours { get; set; } = 1;
    public int MaximumRentalDays { get; set; } = 30;
    public int PickupEarlyMinutes { get; set; } = 60;
    public int MinimumCustomerAge { get; set; } = 18;

    // Branch opening hours are stored in local time, bookings in UTC
    public string TimeZoneId { get; set; } = "India Standard Time";
    public bool EnforceBranchHours { get; set; } = true;
}
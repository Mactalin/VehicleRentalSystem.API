namespace VehicleRentalSystem.Application.Common;

/// Converts UTC times to the branch's local time (default: India Standard Time).
public class BranchClock
{
    private readonly TimeZoneInfo _zone;

    public BranchClock(BillingSettings settings) => _zone = Resolve(settings.TimeZoneId);

    public DateTime ToLocal(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), _zone);

    public DateOnly LocalDate(DateTime utc) => DateOnly.FromDateTime(ToLocal(utc));

    private static TimeZoneInfo Resolve(string id)
    {
        // Windows uses "India Standard Time", Linux uses "Asia/Kolkata"
        foreach (var candidate in new[] { id, "Asia/Kolkata", "India Standard Time" })
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(candidate); }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }
        }
        return TimeZoneInfo.Utc;
    }
}
namespace VehicleRentalSystem.Application.DTOs.BookingDrivers;

public class BookingDriverDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public string DriverPhone { get; set; } = string.Empty;
    public decimal ChargeApplied { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
    public bool IsActive { get; set; }
}
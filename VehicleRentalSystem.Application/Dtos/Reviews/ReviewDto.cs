namespace VehicleRentalSystem.Application.DTOs.Reviews;

public class ReviewDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int? VehicleId { get; set; }
    public int? DriverId { get; set; }
    public string? DriverName { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsVerifiedBooking { get; set; }
    public DateTime PostedAt { get; set; }
}
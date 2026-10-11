namespace VehicleRentalSystem.Application.DTOs.BookingServices;

public class BookingAdditionalServiceDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int AdditionalServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
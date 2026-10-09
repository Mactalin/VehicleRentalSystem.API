

using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Domain.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;

    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public int CustomerId { get; set; }

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

    public decimal SubTotal => Items.Sum(i => i.LineAmount);
    public decimal TaxTotal => Items.Sum(i => i.TaxAmount);
    public decimal GrandTotal => SubTotal + TaxTotal;
}
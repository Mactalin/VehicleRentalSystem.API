namespace VehicleRentalSystem.Domain.Entities;

public class InvoiceItem : BaseEntity
{
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;

    public string Description { get; set; } = string.Empty;   // "Rental (2 days)", "Late fee"
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }                       // e.g. 0.18 for 18% GST, 0 for deposit

    public decimal LineAmount => Quantity * UnitPrice;
    public decimal TaxAmount => Math.Round(LineAmount * TaxRate, 2);
}
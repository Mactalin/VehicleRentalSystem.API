using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Payments
{

    public class InvoiceItemDto
    {
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; }
        public decimal LineAmount { get; set; }
        public decimal TaxAmount { get; set; }
    }
}

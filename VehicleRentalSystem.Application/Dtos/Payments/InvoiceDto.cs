using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Payments
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public int BookingId { get; set; }
        public string BookingNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; }
        public InvoiceStatus Status { get; set; }
        public List<InvoiceItemDto> Items { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal GrandTotal { get; set; }
    }
}

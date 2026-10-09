using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Dtos.Bookings
{
    public class PriceQuoteDto
    {
        public RateType RateType { get; set; }
        public decimal RateApplied { get; set; }
        public int Units { get; set; }                // hours or days
        public decimal BaseAmount { get; set; }
        public decimal AddOnAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal SecurityDeposit { get; set; }
        public decimal AdvanceAmount { get; set; }

    }
}

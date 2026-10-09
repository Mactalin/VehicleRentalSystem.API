using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.Common
{

    public class BillingSettings
    {
        public decimal GstRate { get; set; } = 0.18m;
        public decimal AdvancePercent { get; set; } = 0.30m;          // of rental total
        public int PendingExpiryMinutes { get; set; } = 15;           // unpaid booking hold time

        public decimal LateGraceHours { get; set; } = 1m;
        public decimal LatePenaltyMultiplier { get; set; } = 1.5m;

        public int FullRefundBeforeHours { get; set; } = 24;          // cancel earlier than this = 100% refund
        public decimal PartialRefundPercent { get; set; } = 0.50m;    // otherwise this share of rental payments

        public int MinimumRentalHours { get; set; } = 1;
        public int MaximumRentalDays { get; set; } = 30;
        public int PickupEarlyMinutes { get; set; } = 60;             // pickup allowed this long before start
        public int MinimumCustomerAge { get; set; } = 18;
    }
}

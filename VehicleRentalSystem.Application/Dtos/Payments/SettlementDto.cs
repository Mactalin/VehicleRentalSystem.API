using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Payments
{
    public class SettlementDto
    {
        public int BookingId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal RentalPaid { get; set; }
        public decimal RentalBalance { get; set; }
        public decimal DepositPaid { get; set; }
        public decimal DepositRefunded { get; set; }
        public decimal LateFee { get; set; }
        public decimal DamageCharge { get; set; }
        public decimal LateFeeDue { get; set; }          // not covered by the deposit
        public decimal DamageChargeDue { get; set; }     // not covered by the deposit
        public decimal DepositToRefund { get; set; }
        public decimal TotalDue { get; set; }            // rental balance + charges the deposit could not cover
        public bool IsSettled { get; set; }
    }
}

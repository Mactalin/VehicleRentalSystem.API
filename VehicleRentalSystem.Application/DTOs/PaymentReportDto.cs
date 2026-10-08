using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleRentalSystem.Application.DTOs.Reports;

public class PaymentReportDto
{
    public decimal TotalPayments { get; set; }
    public decimal PendingPayments { get; set; }
    public decimal CompletedPayments { get; set; }
}

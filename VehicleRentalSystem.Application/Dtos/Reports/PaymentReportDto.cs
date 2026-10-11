namespace VehicleRentalSystem.Application.DTOs.Reports;

public class PaymentReportDto
{
    public decimal TotalPayments { get; set; }
    public decimal PendingPayments { get; set; }
    public decimal CompletedPayments { get; set; }
    public int TotalTransactions { get; set; }
    public List<PaymentDataPointDto> DataPoints { get; set; } = new();
}

public class PaymentDataPointDto
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public int Count { get; set; }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VehicleRentalSystem.Application.Dtos.Payments
{

    public class RefundRequestDto
    {
        [Range(1, int.MaxValue)] public int PaymentId { get; set; }
        [Range(0.01, 10000000)] public decimal Amount { get; set; }
        [Required, StringLength(300)] public string Reason { get; set; } = string.Empty;
    }
}

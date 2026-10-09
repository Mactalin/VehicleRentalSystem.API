using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Interface_Service
{
    public record GatewayResult(bool Success, string? Reference, string? FailureReason);

    public interface IPaymentGateway
    {
        Task<GatewayResult> ChargeAsync(decimal amount, string currency, PaymentMethod method,
            string description, CancellationToken ct = default);

        Task<GatewayResult> RefundAsync(string originalReference, decimal amount, CancellationToken ct = default);
    }
}

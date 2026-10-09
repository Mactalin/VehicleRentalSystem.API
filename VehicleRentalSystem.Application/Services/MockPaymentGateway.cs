using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Application.Interface_Service;
using VehicleRentalSystem.Domain.Enums;

namespace VehicleRentalSystem.Application.Services
{
    public class MockPaymentGateway : IPaymentGateway
    {
        public Task<GatewayResult> ChargeAsync(decimal amount, string currency, PaymentMethod method,
            string description, CancellationToken ct = default)
        {
            var prefix = method == PaymentMethod.Cash ? "CASH" : "MOCK";
            var reference = $"{prefix}-{Guid.NewGuid():N}"[..20].ToUpperInvariant();
            return Task.FromResult(new GatewayResult(true, reference, null));
        }

        public Task<GatewayResult> RefundAsync(string originalReference, decimal amount, CancellationToken ct = default)
        {
            var reference = $"RFND-{Guid.NewGuid():N}"[..20].ToUpperInvariant();
            return Task.FromResult(new GatewayResult(true, reference, null));
        }
    }
}

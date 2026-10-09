using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context) { }

        // Tracked and with refunds, because services update payment status and add refunds
        public async Task<IReadOnlyList<Payment>> GetByBookingAsync(int bookingId, CancellationToken ct = default) =>
            await _dbSet
                .Include(p => p.Refunds)
                .Where(p => p.BookingId == bookingId)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync(ct);

        public async Task<IReadOnlyList<Payment>> GetByCustomerAsync(int customerId, CancellationToken ct = default) =>
            await _dbSet.AsNoTracking()
                .Include(p => p.Refunds)
                .Where(p => p.Booking.CustomerId == customerId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(ct);

        public async Task<Payment?> GetByGatewayReferenceAsync(string reference, CancellationToken ct = default) =>
            await _dbSet.Include(p => p.Refunds)
                .FirstOrDefaultAsync(p => p.GatewayReference == reference, ct);
    }
}

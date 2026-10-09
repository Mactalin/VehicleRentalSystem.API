using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(AppDbContext context) : base(context) { }

        public async Task<Invoice?> GetByBookingAsync(int bookingId, CancellationToken ct = default) =>
            await _dbSet.Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.BookingId == bookingId, ct);

        /// Format: INV-202610-000001. Uses the unique index as the safety net against duplicates.
        public async Task<string> GenerateInvoiceNumberAsync(CancellationToken ct = default)
        {
            var prefix = $"INV-{DateTime.UtcNow:yyyyMM}-";

            // IgnoreQueryFilters so soft-deleted invoices still consume their number
            var countThisMonth = await _dbSet.IgnoreQueryFilters()
                .CountAsync(i => i.InvoiceNumber.StartsWith(prefix), ct);

            return $"{prefix}{countThisMonth + 1:D6}";
        }
    }
}

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System.Data;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public ICustomerRepository Customers { get; }
        public IKycRepository KycDocuments { get; }
        public IBookingRepository Bookings { get; }
        public IInspectionRepository Inspections { get; }
        public IPaymentRepository Payments { get; }
        public IInvoiceRepository Invoices { get; }
        public IVehicleRepository Vehicles { get; }

        public UnitOfWork(
            AppDbContext context,
            ICustomerRepository customers,
            IKycRepository kycDocuments,
            IBookingRepository bookings,
            IInspectionRepository inspections,
            IPaymentRepository payments,
            IInvoiceRepository invoices,
            IVehicleRepository vehicles)
        {
            _context = context;
            Customers = customers;
            KycDocuments = kycDocuments;
            Bookings = bookings;
            Inspections = inspections;
            Payments = payments;
            Invoices = invoices;
            Vehicles = vehicles;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
            _context.SaveChangesAsync(ct);

        /// Serializable isolation locks the vehicle/time range, so two customers
        /// cannot both pass the availability check and insert overlapping bookings.
        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction is not null) return;
            _transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            if (_transaction is null)
                throw new InvalidOperationException("No active transaction. Call BeginTransactionAsync first.");

            try
            {
                await _context.SaveChangesAsync(ct);
                await _transaction.CommitAsync(ct);
            }
            catch
            {
                await RollbackAsync(ct);
                throw;
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            if (_transaction is null) return;
            await _transaction.RollbackAsync(ct);
            await DisposeTransactionAsync();
        }

        private async Task DisposeTransactionAsync()
        {
            if (_transaction is null) return;
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}

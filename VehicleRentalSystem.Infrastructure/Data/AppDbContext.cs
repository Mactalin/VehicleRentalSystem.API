using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Common;
using VehicleRentalSystem.Domain.Entities;


namespace VehicleRentalSystem.Infrastructure.Data;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Mac's modules
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<KycDocument> KycDocuments => Set<KycDocument>();
    public DbSet<DrivingLicence> DrivingLicences => Set<DrivingLicence>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingModification> BookingModifications => Set<BookingModification>();
    public DbSet<VehicleInspection> VehicleInspections => Set<VehicleInspection>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Refund> Refunds => Set<Refund>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();

    // Anbu's modules (placeholders, he adds the rest: users, categories, maintenance ...)
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<VehicleImage> VehicleImages => Set<VehicleImage>();
        public DbSet<VehicleDocument> VehicleDocuments => Set<VehicleDocument>();

        public DbSet<VehicleCategory> VehicleCategories => Set<VehicleCategory>();
        public DbSet<CategoryPricing> CategoryPricings => Set<CategoryPricing>();
        public DbSet<SeasonalPricing> SeasonalPricings => Set<SeasonalPricing>();
        public DbSet<AdditionalCharge> AdditionalCharges => Set<AdditionalCharge>();

        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<BranchOperatingHour> BranchOperatingHours => Set<BranchOperatingHour>();
        public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        base.OnModelCreating(modelBuilder);

        // Picks up every IEntityTypeConfiguration<T> in this assembly (one file per entity)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Soft-delete filter on every entity that inherits BaseEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType)) continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var isDeleted = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
            var body = Expression.Equal(isDeleted, Expression.Constant(false));
            entityType.SetQueryFilter(Expression.Lambda(body, parameter));
        }
        }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;

                case EntityState.Deleted:           // convert hard delete into soft delete
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.UpdatedAt = now;
                    break;
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

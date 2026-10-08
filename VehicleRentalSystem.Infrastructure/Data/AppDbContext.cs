using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Common;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();

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
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Modified)
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

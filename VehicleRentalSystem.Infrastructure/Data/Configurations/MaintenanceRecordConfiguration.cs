using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations
{
    public class MaintenanceRecordConfiguration : IEntityTypeConfiguration<MaintenanceRecord>
    {
        public void Configure(EntityTypeBuilder<MaintenanceRecord> builder)
        {
            builder.ToTable("MaintenanceRecords");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Description).IsRequired().HasMaxLength(500);
            builder.Property(m => m.VendorName).HasMaxLength(150);
            builder.Property(m => m.Cost).HasPrecision(18, 2);
            builder.Property(m => m.Type).HasConversion<string>().HasMaxLength(30);
            builder.Property(m => m.Status).HasConversion<string>().HasMaxLength(30);

            builder.HasIndex(m => m.ScheduledDate);

            builder.HasOne(m => m.Vehicle)
                   .WithMany(v => v.MaintenanceRecords)
                   .HasForeignKey(m => m.VehicleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

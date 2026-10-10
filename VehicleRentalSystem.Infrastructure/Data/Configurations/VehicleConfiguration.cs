using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");
            builder.HasKey(v => v.Id);

        builder.Property(v => v.Name).IsRequired().HasMaxLength(150);
            builder.Property(v => v.RegistrationNumber).IsRequired().HasMaxLength(20);
            builder.Property(v => v.Make).IsRequired().HasMaxLength(100);
            builder.Property(v => v.Model).IsRequired().HasMaxLength(100);
            builder.Property(v => v.Color).HasMaxLength(50);

            builder.Property(v => v.FuelType).HasConversion<string>().HasMaxLength(30);
            builder.Property(v => v.Transmission).HasConversion<string>().HasMaxLength(30);
            builder.Property(v => v.Status).HasConversion<string>().HasMaxLength(30);

            builder.HasIndex(v => v.RegistrationNumber).IsUnique();
            builder.HasIndex(v => v.Status);
            builder.HasIndex(v => new { v.BranchId, v.Status });

            builder.HasOne(v => v.Category)
                   .WithMany(c => c.Vehicles)
                   .HasForeignKey(v => v.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Branch)
                   .WithMany(b => b.Vehicles)
                   .HasForeignKey(v => v.BranchId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

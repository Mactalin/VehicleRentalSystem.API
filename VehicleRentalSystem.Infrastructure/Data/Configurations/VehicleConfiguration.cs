using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations;

// PLACEHOLDER: Anbu owns these. Remove this file when his real configurations are merged.
public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name).IsRequired().HasMaxLength(150);
        builder.Property(v => v.RegistrationNumber).IsRequired().HasMaxLength(20);
        builder.Property(v => v.HourlyRate).HasPrecision(18, 2);
        builder.Property(v => v.DailyRate).HasPrecision(18, 2);
        builder.Property(v => v.SecurityDeposit).HasPrecision(18, 2);

        builder.HasIndex(v => v.RegistrationNumber).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(v => new { v.BranchId, v.CategoryId });
    }
}

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name).IsRequired().HasMaxLength(150);
        builder.Property(b => b.City).IsRequired().HasMaxLength(100);
    }
}
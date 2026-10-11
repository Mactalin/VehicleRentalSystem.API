using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> b)
    {
        b.ToTable("Drivers");
        b.HasKey(x => x.Id);

        b.Property(x => x.FullName).IsRequired().HasMaxLength(100);
        b.Property(x => x.LicenseNumber).IsRequired().HasMaxLength(50);
        b.Property(x => x.Phone).IsRequired().HasMaxLength(20);
        b.Property(x => x.Email).HasMaxLength(100);
        b.Property(x => x.Notes).HasMaxLength(500);
        b.Property(x => x.ChargePerDay).HasColumnType("decimal(18,2)");
        b.Property(x => x.ChargePerHour).HasColumnType("decimal(18,2)");

        b.HasIndex(x => x.LicenseNumber).IsUnique();
        b.HasIndex(x => x.Status);
        b.HasIndex(x => x.IsActive);
    }
}
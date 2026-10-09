using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations;

public class DrivingLicenceConfiguration : IEntityTypeConfiguration<DrivingLicence>
{
    public void Configure(EntityTypeBuilder<DrivingLicence> builder)
    {
        builder.ToTable("DrivingLicences");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.LicenceNumber).IsRequired().HasMaxLength(30);
        builder.Property(d => d.IssuingAuthority).HasMaxLength(150);
        builder.Property(d => d.VehicleClasses).HasMaxLength(100);
        builder.Property(d => d.FilePath).IsRequired().HasMaxLength(500);
        builder.Property(d => d.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(d => d.RejectionReason).HasMaxLength(500);

        builder.HasOne(d => d.Customer)
               .WithMany(c => c.DrivingLicences)
               .HasForeignKey(d => d.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => d.LicenceNumber).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}
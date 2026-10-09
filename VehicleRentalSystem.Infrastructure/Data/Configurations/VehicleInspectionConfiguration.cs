using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations;

public class VehicleInspectionConfiguration : IEntityTypeConfiguration<VehicleInspection>
{
    public void Configure(EntityTypeBuilder<VehicleInspection> builder)
    {
        builder.ToTable("VehicleInspections");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Type).HasConversion<string>().HasMaxLength(10);
        builder.Property(i => i.FuelLevelPercent).HasPrecision(5, 2);
        builder.Property(i => i.DamageCharge).HasPrecision(18, 2);
        builder.Property(i => i.DamageNotes).HasMaxLength(1000);
        builder.Property(i => i.PhotoUrls).HasMaxLength(2000);

        builder.HasOne(i => i.Booking)
               .WithMany(b => b.Inspections)
               .HasForeignKey(i => i.BookingId)
               .OnDelete(DeleteBehavior.Cascade);

        // One PICKUP and one RETURN inspection per booking
        builder.HasIndex(i => new { i.BookingId, i.Type }).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}
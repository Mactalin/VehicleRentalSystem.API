using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations;

public class BookingDriverConfiguration : IEntityTypeConfiguration<BookingDriver>
{
    public void Configure(EntityTypeBuilder<BookingDriver> b)
    {
        b.ToTable("BookingDrivers");
        b.HasKey(x => x.Id);

        b.Property(x => x.ChargeApplied).HasColumnType("decimal(18,2)");

        b.HasOne(x => x.Booking)
            .WithMany()
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Driver)
            .WithMany(d => d.BookingAssignments)
            .HasForeignKey(x => x.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.BookingId);
        b.HasIndex(x => x.DriverId);
        b.HasIndex(x => x.IsActive);
    }
}
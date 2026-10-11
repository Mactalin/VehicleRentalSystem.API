using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations;

public class BookingAdditionalServiceConfiguration : IEntityTypeConfiguration<BookingAdditionalService>
{
    public void Configure(EntityTypeBuilder<BookingAdditionalService> b)
    {
        b.ToTable("BookingAdditionalServices");
        b.HasKey(x => x.Id);

        b.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
        b.Property(x => x.LineTotal).HasColumnType("decimal(18,2)");

        b.HasOne(x => x.Booking)
            .WithMany()
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.AdditionalService)
            .WithMany(s => s.BookingAssignments)
            .HasForeignKey(x => x.AdditionalServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.BookingId);
        b.HasIndex(x => x.AdditionalServiceId);
    }
}
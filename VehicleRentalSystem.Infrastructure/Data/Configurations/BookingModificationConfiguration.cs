using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations;

public class BookingModificationConfiguration : IEntityTypeConfiguration<BookingModification>
{
    public void Configure(EntityTypeBuilder<BookingModification> builder)
    {
        builder.ToTable("BookingModifications");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.PriceDifference).HasPrecision(18, 2);
        builder.Property(m => m.Reason).HasMaxLength(500);

        builder.HasOne(m => m.Booking)
               .WithMany(b => b.Modifications)
               .HasForeignKey(m => m.BookingId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => m.BookingId);
    }
}
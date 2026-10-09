using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Method).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Amount).HasPrecision(18, 2);
        builder.Property(p => p.Currency).IsRequired().HasMaxLength(3);
        builder.Property(p => p.GatewayReference).HasMaxLength(100);
        builder.Property(p => p.FailureReason).HasMaxLength(300);

        builder.HasOne(p => p.Booking)
               .WithMany(b => b.Payments)
               .HasForeignKey(p => p.BookingId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.BookingId);
        builder.HasIndex(p => p.GatewayReference);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations;

public class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        builder.ToTable("Refunds");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Amount).HasPrecision(18, 2);
        builder.Property(r => r.Reason).IsRequired().HasMaxLength(300);
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.GatewayReference).HasMaxLength(100);

        builder.HasOne(r => r.Payment)
               .WithMany(p => p.Refunds)
               .HasForeignKey(r => r.PaymentId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Booking>().WithMany()
               .HasForeignKey(r => r.BookingId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.PaymentId);
        builder.HasIndex(r => r.BookingId);
    }
}
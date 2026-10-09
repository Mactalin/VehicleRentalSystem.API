using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.BookingNumber).IsRequired().HasMaxLength(30);
        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(b => b.RateType).HasConversion<string>().HasMaxLength(10);
        builder.Property(b => b.CouponCode).HasMaxLength(50);
        builder.Property(b => b.CancellationReason).HasMaxLength(500);

        // Money: price snapshot and settlement
        builder.Property(b => b.RateApplied).HasPrecision(18, 2);
        builder.Property(b => b.BaseAmount).HasPrecision(18, 2);
        builder.Property(b => b.AddOnAmount).HasPrecision(18, 2);
        builder.Property(b => b.DiscountAmount).HasPrecision(18, 2);
        builder.Property(b => b.TaxAmount).HasPrecision(18, 2);
        builder.Property(b => b.TotalAmount).HasPrecision(18, 2);
        builder.Property(b => b.SecurityDeposit).HasPrecision(18, 2);
        builder.Property(b => b.AdvanceAmount).HasPrecision(18, 2);
        builder.Property(b => b.LateFee).HasPrecision(18, 2);
        builder.Property(b => b.DamageCharge).HasPrecision(18, 2);
        builder.Property(b => b.LateGraceHours).HasPrecision(5, 2);
        builder.Property(b => b.LatePenaltyMultiplier).HasPrecision(5, 2);

        // Relationships (financial records are never cascade-deleted)
        builder.HasOne(b => b.Customer)
               .WithMany(c => c.Bookings)
               .HasForeignKey(b => b.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Vehicle)
               .WithMany()
               .HasForeignKey(b => b.VehicleId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Branch>().WithMany()
               .HasForeignKey(b => b.PickupBranchId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Branch>().WithMany()
               .HasForeignKey(b => b.DropBranchId)
               .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(b => b.BookingNumber).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(b => new { b.VehicleId, b.StartAt, b.EndAt });   // availability check
        builder.HasIndex(b => new { b.CustomerId, b.Status });            // rental history
        builder.HasIndex(b => new { b.Status, b.PendingExpiresAt });      // expiry job
    }
}
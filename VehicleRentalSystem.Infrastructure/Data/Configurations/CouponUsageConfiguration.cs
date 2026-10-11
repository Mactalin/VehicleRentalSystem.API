using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations;

public class CouponUsageConfiguration : IEntityTypeConfiguration<CouponUsage>
{
    public void Configure(EntityTypeBuilder<CouponUsage> b)
    {
        b.ToTable("CouponUsages");
        b.HasKey(x => x.Id);

        b.Property(x => x.DiscountApplied).HasColumnType("decimal(18,2)");

        b.HasOne(x => x.Coupon)
            .WithMany(c => c.Usages)
            .HasForeignKey(x => x.CouponId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Booking)
            .WithMany()
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.CouponId);
        b.HasIndex(x => x.CustomerId);
        b.HasIndex(x => new { x.CouponId, x.CustomerId });
    }
}
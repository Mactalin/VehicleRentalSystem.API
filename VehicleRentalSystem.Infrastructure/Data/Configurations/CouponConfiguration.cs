using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> b)
    {
        b.ToTable("Coupons");
        b.HasKey(x => x.Id);

        b.Property(x => x.Code).IsRequired().HasMaxLength(50);
        b.Property(x => x.Description).HasMaxLength(500);
        b.Property(x => x.DiscountValue).HasColumnType("decimal(18,2)");
        b.Property(x => x.MaxDiscountAmount).HasColumnType("decimal(18,2)");
        b.Property(x => x.MinOrderAmount).HasColumnType("decimal(18,2)");

        b.HasIndex(x => x.Code).IsUnique();
        b.HasIndex(x => x.Status);
        b.HasIndex(x => x.IsActive);
        b.HasIndex(x => new { x.ValidFrom, x.ValidTo });
    }
}
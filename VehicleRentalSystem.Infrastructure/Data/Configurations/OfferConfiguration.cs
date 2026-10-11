using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations;

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> b)
    {
        b.ToTable("Offers");
        b.HasKey(x => x.Id);

        b.Property(x => x.Title).IsRequired().HasMaxLength(100);
        b.Property(x => x.Description).HasMaxLength(500);
        b.Property(x => x.DiscountPercentage).HasColumnType("decimal(5,2)");
        b.Property(x => x.MaxDiscountAmount).HasColumnType("decimal(18,2)");
        b.Property(x => x.BannerImageUrl).HasMaxLength(500);

        b.HasIndex(x => x.IsActive);
        b.HasIndex(x => new { x.StartDate, x.EndDate });
    }
}
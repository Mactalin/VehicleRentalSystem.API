using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations
{
    public class CategoryPricingConfiguration : IEntityTypeConfiguration<CategoryPricing>
    {
        public void Configure(EntityTypeBuilder<CategoryPricing> builder)
        {
            builder.ToTable("CategoryPricings");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.HourlyPrice).HasPrecision(18, 2);
            builder.Property(p => p.DailyPrice).HasPrecision(18, 2);
            builder.Property(p => p.WeeklyPrice).HasPrecision(18, 2);
            builder.Property(p => p.SecurityDeposit).HasPrecision(18, 2);

            builder.HasOne(p => p.Category)
                   .WithOne(c => c.Pricing)
                   .HasForeignKey<CategoryPricing>(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.CategoryId).IsUnique();
        }
    }
}

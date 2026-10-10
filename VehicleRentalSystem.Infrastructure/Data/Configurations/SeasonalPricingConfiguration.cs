using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations
{
    public class SeasonalPricingConfiguration : IEntityTypeConfiguration<SeasonalPricing>
    {
        public void Configure(EntityTypeBuilder<SeasonalPricing> builder)
        {
            builder.ToTable("SeasonalPricings");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Multiplier).HasPrecision(5, 2);

            builder.HasOne(s => s.Category)
                   .WithMany(c => c.SeasonalPricings)
                   .HasForeignKey(s => s.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => new { s.CategoryId, s.StartDate, s.EndDate });
        }
    }
}

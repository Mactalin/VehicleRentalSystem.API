using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations
{
    public class AdditionalChargeConfiguration : IEntityTypeConfiguration<AdditionalCharge>
    {
        public void Configure(EntityTypeBuilder<AdditionalCharge> builder)
        {
            builder.ToTable("AdditionalCharges");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Description).HasMaxLength(300);
            builder.Property(a => a.Amount).HasPrecision(18, 2);
            builder.Property(a => a.ChargeType).HasConversion<string>().HasMaxLength(30);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations
{
    public class VehicleImageConfiguration : IEntityTypeConfiguration<VehicleImage>
    {
        public void Configure(EntityTypeBuilder<VehicleImage> builder)
        {
            builder.ToTable("VehicleImages");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.ImageUrl).IsRequired().HasMaxLength(500);

            builder.HasOne(i => i.Vehicle)
                   .WithMany(v => v.Images)
                   .HasForeignKey(i => i.VehicleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.ToTable("Branches");
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Name).IsRequired().HasMaxLength(150);
            builder.Property(b => b.Address).IsRequired().HasMaxLength(300);
            builder.Property(b => b.City).IsRequired().HasMaxLength(100);
            builder.Property(b => b.State).IsRequired().HasMaxLength(100);
            builder.Property(b => b.Pincode).IsRequired().HasMaxLength(10);
            builder.Property(b => b.Phone).HasMaxLength(20);

            builder.HasIndex(b => b.City);
        }
    }
}

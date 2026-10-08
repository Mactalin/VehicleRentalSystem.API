using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations
{
    public class BranchOperatingHourConfiguration : IEntityTypeConfiguration<BranchOperatingHour>
    {
        public void Configure(EntityTypeBuilder<BranchOperatingHour> builder)
        {
            builder.ToTable("BranchOperatingHours");
            builder.HasKey(h => h.Id);

            builder.HasOne(h => h.Branch)
                   .WithMany(b => b.OperatingHours)
                   .HasForeignKey(h => h.BranchId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(h => new { h.BranchId, h.DayOfWeek }).IsUnique();
        }
    }
}

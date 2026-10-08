using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations
{
    public class VehicleDocumentConfiguration : IEntityTypeConfiguration<VehicleDocument>
    {
        public void Configure(EntityTypeBuilder<VehicleDocument> builder)
        {
            builder.ToTable("VehicleDocuments");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.DocumentType).HasConversion<string>().HasMaxLength(50);
            builder.Property(d => d.DocumentNumber).IsRequired().HasMaxLength(100);
            builder.Property(d => d.FileUrl).HasMaxLength(500);

            builder.HasIndex(d => d.ExpiryDate);

            builder.HasOne(d => d.Vehicle)
                   .WithMany(v => v.Documents)
                   .HasForeignKey(d => d.VehicleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

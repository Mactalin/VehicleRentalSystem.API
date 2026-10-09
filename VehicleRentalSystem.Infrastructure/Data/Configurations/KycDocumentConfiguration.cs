using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations;

public class KycDocumentConfiguration : IEntityTypeConfiguration<KycDocument>
{
    public void Configure(EntityTypeBuilder<KycDocument> builder)
    {
        builder.ToTable("KycDocuments");
        builder.HasKey(k => k.Id);

        builder.Property(k => k.DocumentType).HasConversion<string>().HasMaxLength(20);
        builder.Property(k => k.DocumentNumber).IsRequired().HasMaxLength(50);
        builder.Property(k => k.FilePath).IsRequired().HasMaxLength(500);
        builder.Property(k => k.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(k => k.RejectionReason).HasMaxLength(500);

        builder.HasOne(k => k.Customer)
               .WithMany(c => c.KycDocuments)
               .HasForeignKey(k => k.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(k => new { k.CustomerId, k.DocumentType });
    }
}
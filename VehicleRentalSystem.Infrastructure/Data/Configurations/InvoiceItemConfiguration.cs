using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations;

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Description).IsRequired().HasMaxLength(200);
        builder.Property(i => i.UnitPrice).HasPrecision(18, 2);
        builder.Property(i => i.TaxRate).HasPrecision(5, 4);

        // LineAmount and TaxAmount are get-only, so EF does not map them

        builder.HasOne(i => i.Invoice)
               .WithMany(inv => inv.Items)
               .HasForeignKey(i => i.InvoiceId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => i.InvoiceId);
    }
}
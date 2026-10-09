using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(30);
        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);

        // One invoice per booking
        builder.HasOne(i => i.Booking)
               .WithOne(b => b.Invoice)
               .HasForeignKey<Invoice>(i => i.BookingId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Customer>().WithMany()
               .HasForeignKey(i => i.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => i.InvoiceNumber).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(i => i.BookingId).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}
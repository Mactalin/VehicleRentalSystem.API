using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data.Configurations;

public class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
{
    public void Configure(EntityTypeBuilder<Complaint> b)
    {
        b.ToTable("Complaints");
        b.HasKey(x => x.Id);

        b.Property(x => x.Category).IsRequired().HasMaxLength(100);
        b.Property(x => x.Description).IsRequired().HasMaxLength(2000);
        b.Property(x => x.AdminResponse).HasMaxLength(2000);

        b.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Booking)
            .WithMany()
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasIndex(x => x.Status);
        b.HasIndex(x => x.CustomerId);
        b.HasIndex(x => x.BookingId);
    }
}
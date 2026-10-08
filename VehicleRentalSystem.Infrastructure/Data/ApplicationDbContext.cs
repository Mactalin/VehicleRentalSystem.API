using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Entities;

namespace VehicleRentalSystem.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // ----- Modules 9–12 (Lenin) -----
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<AdditionalService> AdditionalServices { get; set; }
    public DbSet<BookingDriver> BookingDrivers { get; set; }
    public DbSet<BookingAdditionalService> BookingAdditionalServices { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Complaint> Complaints { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    // ----- Stub entities (replace when teammates finish their modules) -----
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==================================================
        // GLOBAL CONFIGURATION
        // ==================================================

        // All decimal properties default to decimal(18,2) unless overridden
        foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        // ==================================================
        // COMPOSITE KEYS (JOIN TABLES)
        // ==================================================

        modelBuilder.Entity<BookingDriver>()
            .HasKey(bd => new { bd.BookingId, bd.DriverId });

        modelBuilder.Entity<BookingAdditionalService>()
            .HasKey(bas => new { bas.BookingId, bas.AdditionalServiceId });

        // ==================================================
        // UNIQUE INDEXES
        // ==================================================

        modelBuilder.Entity<Coupon>()
            .HasIndex(c => c.Code)
            .IsUnique();

        modelBuilder.Entity<Driver>()
            .HasIndex(d => d.LicenseNumber)
            .IsUnique();

        // ==================================================
        // RELATIONSHIPS & DELETE BEHAVIOR
        // ==================================================

        // --- BookingDriver -> Booking ---
        modelBuilder.Entity<BookingDriver>()
            .HasOne(bd => bd.Booking)
            .WithMany()
            .HasForeignKey(bd => bd.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- BookingDriver -> Driver ---
        modelBuilder.Entity<BookingDriver>()
            .HasOne(bd => bd.Driver)
            .WithMany(d => d.BookingDrivers)
            .HasForeignKey(bd => bd.DriverId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a Driver with active assignments

        // --- BookingAdditionalService -> Booking ---
        modelBuilder.Entity<BookingAdditionalService>()
            .HasOne(bas => bas.Booking)
            .WithMany()
            .HasForeignKey(bas => bas.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- BookingAdditionalService -> AdditionalService ---
        modelBuilder.Entity<BookingAdditionalService>()
            .HasOne(bas => bas.AdditionalService)
            .WithMany(s => s.BookingAdditionalServices)
            .HasForeignKey(bas => bas.AdditionalServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        // --- Review -> Booking ---
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Booking)
            .WithMany()
            .HasForeignKey(r => r.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- Review -> Customer ---
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // --- Review -> Vehicle (optional) ---
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Vehicle)
            .WithMany()
            .HasForeignKey(r => r.VehicleId)
            .OnDelete(DeleteBehavior.SetNull);

        // --- Review -> Driver (optional) ---
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Driver)
            .WithMany()
            .HasForeignKey(r => r.DriverId)
            .OnDelete(DeleteBehavior.SetNull);

        // --- Complaint -> Customer ---
        modelBuilder.Entity<Complaint>()
            .HasOne(c => c.Customer)
            .WithMany()
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // --- Complaint -> Booking (optional) ---
        modelBuilder.Entity<Complaint>()
            .HasOne(c => c.Booking)
            .WithMany()
            .HasForeignKey(c => c.BookingId)
            .OnDelete(DeleteBehavior.SetNull);

        // --- Notification -> User ---
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ==================================================
        // INDEXES ON FOREIGN KEYS (query performance)
        // ==================================================

        modelBuilder.Entity<Review>().HasIndex(r => r.BookingId);
        modelBuilder.Entity<Review>().HasIndex(r => r.CustomerId);
        modelBuilder.Entity<Review>().HasIndex(r => r.VehicleId);
        modelBuilder.Entity<Review>().HasIndex(r => r.DriverId);

        modelBuilder.Entity<Complaint>().HasIndex(c => c.CustomerId);
        modelBuilder.Entity<Complaint>().HasIndex(c => c.BookingId);
        modelBuilder.Entity<Complaint>().HasIndex(c => c.Status);

        modelBuilder.Entity<Notification>().HasIndex(n => n.UserId);
        modelBuilder.Entity<Notification>().HasIndex(n => n.IsRead);
        modelBuilder.Entity<Notification>().HasIndex(n => n.CreatedAt);

        // ==================================================
        // APPLY EXTERNAL CONFIGURATIONS
        // ==================================================

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
using Microsoft.EntityFrameworkCore;
using XYZ.DriversService.Domain.Entities;
using XYZ.DriversService.Domain.Enums;

namespace XYZ.DriversService.Infrastructure.Persistence;

public class DriversDbContext : DbContext
{
    public DriversDbContext(DbContextOptions<DriversDbContext> options)
        : base(options)
    {
    }

    public DbSet<Driver> Drivers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Driver entity configuration
        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.DocumentNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LicenseNumber)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(e => e.LicenseCategory)
                .HasConversion<int>();

            entity.Property(e => e.DriverType)
                .HasConversion<int>();

            entity.Property(e => e.Status)
                .HasConversion<int>()
                .HasDefaultValue(DriverStatus.Active);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.IsAssigned)
                .HasDefaultValue(false);

            // Unique constraints
            entity.HasIndex(e => e.DocumentNumber)
                .IsUnique();

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.HasIndex(e => e.LicenseNumber)
                .IsUnique();
        });
    }
}
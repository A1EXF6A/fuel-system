using Microsoft.EntityFrameworkCore;
using XYZ.VehiclesService.Domain.Entities;

namespace XYZ.VehiclesService.Infrastructure.Persistence;

public class VehiclesDbContext : DbContext
{
    public VehiclesDbContext(DbContextOptions<VehiclesDbContext> options) : base(options) { }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<VehicleType> VehicleTypes => Set<VehicleType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Placa única
        modelBuilder.Entity<Vehicle>()
            .HasIndex(v => v.Placa)
            .IsUnique();

        // AssignedDriverDocument opcional
        modelBuilder.Entity<Vehicle>()
            .Property(v => v.AssignedDriverDocument)
            .HasMaxLength(100)
            .IsRequired(false);
    }
}

using Microsoft.EntityFrameworkCore;
using XYZ.VehiclesService.Domain.Entities;

namespace XYZ.VehiclesService.Infrastructure.Persistence;

public class VehiclesDbContext : DbContext
{
    public VehiclesDbContext(DbContextOptions<VehiclesDbContext> options) : base(options) { }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<VehicleType> VehicleTypes => Set<VehicleType>();
}

using Microsoft.EntityFrameworkCore;
using XYZ.FuelService.Domain.Entities;

namespace XYZ.FuelService.Infrastructure.Persistence;

public class FuelDbContext : DbContext
{
    public FuelDbContext(DbContextOptions<FuelDbContext> options) : base(options) { }

    public DbSet<FuelRecord> FuelRecords => Set<FuelRecord>();
}

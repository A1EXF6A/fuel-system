using Microsoft.EntityFrameworkCore;
using XYZ.RoutesService.Domain.Entities;
using Route = XYZ.RoutesService.Domain.Entities.Route;

namespace XYZ.RoutesService.Infrastructure.Persistence;

public class RoutesDbContext : DbContext
{
    public RoutesDbContext(DbContextOptions<RoutesDbContext> options) : base(options) { }

    public DbSet<Route> Routes => Set<Route>();
}

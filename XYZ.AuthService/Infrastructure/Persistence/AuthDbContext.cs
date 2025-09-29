using Microsoft.EntityFrameworkCore;
using XYZ.AuthService.Domain.Entities;

namespace XYZ.AuthService.Infrastructure.Persistence;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
}

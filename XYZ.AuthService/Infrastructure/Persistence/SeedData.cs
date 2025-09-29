using System.Linq;
using XYZ.AuthService.Domain.Entities;
using XYZ.AuthService.Domain.Enums;
using XYZ.AuthService.Infrastructure.Security;

namespace XYZ.AuthService.Infrastructure.Persistence;

public static class SeedData
{
    public static void Initialize(AuthDbContext context, PasswordHasher hasher)
    {
        if (!context.Users.Any())
        {
            context.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = hasher.HashPassword("admin123"),
                Role = UserRole.Admin
            });
            context.SaveChanges();
        }
    }
}

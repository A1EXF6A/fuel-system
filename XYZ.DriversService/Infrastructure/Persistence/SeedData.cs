using Microsoft.EntityFrameworkCore;
using XYZ.DriversService.Domain.Entities;
using XYZ.DriversService.Domain.Enums;

namespace XYZ.DriversService.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task SeedAsync(DriversDbContext context)
    {
        if (!await context.Drivers.AnyAsync())
        {
            var drivers = new List<Driver>
            {
                new Driver
                {
                    FirstName = "Juan",
                    LastName = "Pérez",
                    DocumentNumber = "12345678",
                    PhoneNumber = "+1234567890",
                    Email = "juan.perez@company.com",
                    LicenseNumber = "LIC001",
                    LicenseCategory = LicenseCategory.B,
                    LicenseExpiryDate = DateTime.UtcNow.AddYears(3),
                    DriverType = DriverType.LightMachinery,
                    Status = DriverStatus.Active,
                    HireDate = DateTime.UtcNow.AddYears(-2),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Driver
                {
                    FirstName = "María",
                    LastName = "González",
                    DocumentNumber = "87654321",
                    PhoneNumber = "+1234567891",
                    Email = "maria.gonzalez@company.com",
                    LicenseNumber = "LIC002",
                    LicenseCategory = LicenseCategory.C,
                    LicenseExpiryDate = DateTime.UtcNow.AddYears(2),
                    DriverType = DriverType.HeavyMachinery,
                    Status = DriverStatus.Active,
                    HireDate = DateTime.UtcNow.AddYears(-1),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Driver
                {
                    FirstName = "Carlos",
                    LastName = "Rodríguez",
                    DocumentNumber = "11223344",
                    PhoneNumber = "+1234567892",
                    Email = "carlos.rodriguez@company.com",
                    LicenseNumber = "LIC003",
                    LicenseCategory = LicenseCategory.E,
                    LicenseExpiryDate = DateTime.UtcNow.AddYears(4),
                    DriverType = DriverType.HeavyMachinery,
                    Status = DriverStatus.Active,
                    HireDate = DateTime.UtcNow.AddMonths(-6),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await context.Drivers.AddRangeAsync(drivers);
            await context.SaveChangesAsync();
        }
    }
}
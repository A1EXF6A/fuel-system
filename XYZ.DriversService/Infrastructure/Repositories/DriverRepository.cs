using Microsoft.EntityFrameworkCore;
using XYZ.DriversService.Domain.Entities;
using XYZ.DriversService.Domain.Enums;
using XYZ.DriversService.Infrastructure.Persistence;

namespace XYZ.DriversService.Infrastructure.Repositories;

public class DriverRepository
{
    private readonly DriversDbContext _context;

    public DriverRepository(DriversDbContext context)
    {
        _context = context;
    }

    public async Task<Driver?> GetByIdAsync(int id)
    {
        return await _context.Drivers.FindAsync(id);
    }

    public async Task<Driver?> GetByDocumentNumberAsync(string documentNumber)
    {
        return await _context.Drivers
            .FirstOrDefaultAsync(d => d.DocumentNumber == documentNumber);
    }

    public async Task<Driver?> GetByEmailAsync(string email)
    {
        return await _context.Drivers
            .FirstOrDefaultAsync(d => d.Email == email);
    }

    public async Task<Driver?> GetByLicenseNumberAsync(string licenseNumber)
    {
        return await _context.Drivers
            .FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber);
    }

    public async Task<List<Driver>> GetAllAsync()
    {
        return await _context.Drivers
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .ToListAsync();
    }

    public async Task<List<Driver>> GetByStatusAsync(DriverStatus status)
    {
        return await _context.Drivers
            .Where(d => d.Status == status)
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .ToListAsync();
    }

    public async Task<List<Driver>> GetAvailableAsync()
    {
        return await _context.Drivers
            .Where(d => d.Status == DriverStatus.Active && !d.IsAssigned)
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .ToListAsync();
    }

    public async Task<List<Driver>> GetByDriverTypeAsync(DriverType driverType)
    {
        return await _context.Drivers
            .Where(d => d.DriverType == driverType && d.Status == DriverStatus.Active)
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .ToListAsync();
    }

    public async Task<Driver> CreateAsync(Driver driver)
    {
        driver.CreatedAt = DateTime.UtcNow;
        driver.UpdatedAt = DateTime.UtcNow;
        
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();
        
        return driver;
    }

    public async Task<Driver> UpdateAsync(Driver driver)
    {
        driver.UpdatedAt = DateTime.UtcNow;
        
        _context.Drivers.Update(driver);
        await _context.SaveChangesAsync();
        
        return driver;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var driver = await GetByIdAsync(id);
        if (driver == null)
            return false;

        _context.Drivers.Remove(driver);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Drivers.AnyAsync(d => d.Id == id);
    }

    public async Task<bool> DocumentNumberExistsAsync(string documentNumber, int? excludeId = null)
    {
        var query = _context.Drivers.Where(d => d.DocumentNumber == documentNumber);
        
        if (excludeId.HasValue)
            query = query.Where(d => d.Id != excludeId.Value);
            
        return await query.AnyAsync();
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        var query = _context.Drivers.Where(d => d.Email == email);
        
        if (excludeId.HasValue)
            query = query.Where(d => d.Id != excludeId.Value);
            
        return await query.AnyAsync();
    }

    public async Task<bool> LicenseNumberExistsAsync(string licenseNumber, int? excludeId = null)
    {
        var query = _context.Drivers.Where(d => d.LicenseNumber == licenseNumber);
        
        if (excludeId.HasValue)
            query = query.Where(d => d.Id != excludeId.Value);
            
        return await query.AnyAsync();
    }
}
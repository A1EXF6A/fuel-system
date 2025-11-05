using Microsoft.EntityFrameworkCore;
using XYZ.VehiclesService.Domain.Entities;
using XYZ.VehiclesService.Infrastructure.Persistence;

namespace XYZ.VehiclesService.Infrastructure.Repositories;

public class VehicleRepository
{
    private readonly VehiclesDbContext _db;

    public VehicleRepository(VehiclesDbContext db) => _db = db;

    public async Task<Vehicle> AddAsync(Vehicle v)
    {
        _db.Vehicles.Add(v);
        await _db.SaveChangesAsync();
        return v;
    }

    public async Task<Vehicle?> GetByIdAsync(int id)
        => await _db.Vehicles.Include(x => x.VehicleType).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Vehicle?> GetByPlacaAsync(string placa)
        => await _db.Vehicles.Include(x => x.VehicleType).FirstOrDefaultAsync(x => x.Placa == placa);

    public async Task<List<Vehicle>> GetAllAsync()
        => await _db.Vehicles.Include(x => x.VehicleType).ToListAsync();

    public async Task<Vehicle?> UpdateAssignedDriverDocumentAsync(string placa, string? driverDocument)
    {
        var v = await _db.Vehicles.FirstOrDefaultAsync(x => x.Placa == placa);
        if (v == null) return null;
        v.AssignedDriverDocument = string.IsNullOrWhiteSpace(driverDocument) ? null : driverDocument;
        await _db.SaveChangesAsync();
        return v;
    }

    public async Task<Vehicle?> UpdateAsync(Vehicle vehicle)
    {
        var v = await _db.Vehicles.FirstOrDefaultAsync(x => x.Id == vehicle.Id);
        if (v == null) return null;

        // Update fields (ignore navigation property VehicleType here)
        v.Placa = vehicle.Placa;
        v.Chasis = vehicle.Chasis;
        v.Marca = vehicle.Marca;
        v.Modelo = vehicle.Modelo;
        v.Anio = vehicle.Anio;
        v.VehicleTypeId = vehicle.VehicleTypeId;
        v.Estado = vehicle.Estado;
        v.Km = vehicle.Km;
        v.LastMaintenance = vehicle.LastMaintenance;
        v.AssignedDriverDocument = string.IsNullOrWhiteSpace(vehicle.AssignedDriverDocument) ? null : vehicle.AssignedDriverDocument;

        await _db.SaveChangesAsync();
        return v;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var v = await _db.Vehicles.FirstOrDefaultAsync(x => x.Id == id);
        if (v == null) return false;
        _db.Vehicles.Remove(v);
        await _db.SaveChangesAsync();
        return true;
    }
}

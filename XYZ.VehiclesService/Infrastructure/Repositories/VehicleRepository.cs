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
}

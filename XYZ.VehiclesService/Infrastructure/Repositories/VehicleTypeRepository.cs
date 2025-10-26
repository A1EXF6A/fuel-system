using Microsoft.EntityFrameworkCore;
using XYZ.VehiclesService.Domain.Entities;
using XYZ.VehiclesService.Infrastructure.Persistence;

namespace XYZ.VehiclesService.Infrastructure.Repositories;

public class VehicleTypeRepository
{
    private readonly VehiclesDbContext _db;
    public VehicleTypeRepository(VehiclesDbContext db) => _db = db;

    public async Task<List<VehicleType>> GetAllAsync() => await _db.VehicleTypes.ToListAsync();
}

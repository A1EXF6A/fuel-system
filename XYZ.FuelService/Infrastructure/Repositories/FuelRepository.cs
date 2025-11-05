using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using XYZ.FuelService.Domain.Entities;
using XYZ.FuelService.Infrastructure.Persistence;

namespace XYZ.FuelService.Infrastructure.Repositories;

public class FuelRepository
{
    private readonly FuelDbContext _db;
    public FuelRepository(FuelDbContext db) => _db = db;

    public async Task<FuelRecord> AddAsync(FuelRecord record)
    {
        _db.FuelRecords.Add(record);
        await _db.SaveChangesAsync();
        return record;
    }

    public async Task<FuelRecord?> GetByIdAsync(int id) => await _db.FuelRecords.FindAsync(id);

    public async Task<List<FuelRecord>> GetByFilterAsync(string filterType, string filterValue)
    {
        return filterType switch
        {
            "vehicle" => await _db.FuelRecords.Where(f => f.VehiclePlaca == filterValue).ToListAsync(),
            "driver" when int.TryParse(filterValue, out var dId) => await _db.FuelRecords.Where(f => f.DriverId == dId).ToListAsync(),
            "route" when int.TryParse(filterValue, out var rId) => await _db.FuelRecords.Where(f => f.RouteId == rId).ToListAsync(),
            _ => new List<FuelRecord>()
        };
    }

    public async Task<List<FuelRecord>> GetAllAsync()
    {
        return await _db.FuelRecords.ToListAsync();
    }

    public async Task UpdateAsync(FuelRecord record)
    {
        _db.FuelRecords.Update(record);
        await _db.SaveChangesAsync();
    }

    public async Task<FuelRecord?> UpdateStatusAsync(int id, string estado)
    {
        var record = await _db.FuelRecords.FirstOrDefaultAsync(r => r.Id == id);
        if (record == null) return null;
        record.Estado = Enum.Parse<XYZ.FuelService.Domain.Enums.EstadoConsumo>(estado, true);
        await _db.SaveChangesAsync();
        return record;
    }
}

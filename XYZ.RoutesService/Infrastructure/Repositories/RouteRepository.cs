using Microsoft.EntityFrameworkCore;
using XYZ.RoutesService.Domain.Entities;
using Route = XYZ.RoutesService.Domain.Entities.Route;
using XYZ.RoutesService.Infrastructure.Persistence;

namespace XYZ.RoutesService.Infrastructure.Repositories;

public class RouteRepository
{
    private readonly RoutesDbContext _db;
    public RouteRepository(RoutesDbContext db) => _db = db;

    public async Task<Route> AddAsync(Route route)
    {
        _db.Routes.Add(route);
        await _db.SaveChangesAsync();
        return route;
    }

    public async Task<Route?> GetByIdAsync(int id) => await _db.Routes.FirstOrDefaultAsync(r => r.Id == id);
    public async Task<List<Route>> GetAllAsync() => await _db.Routes.ToListAsync();

    public async Task<Route?> UpdateStatusAsync(int id, string estado)
    {
        var route = await _db.Routes.FirstOrDefaultAsync(r => r.Id == id);
        if (route == null) return null;
        route.Estado = Enum.Parse<XYZ.RoutesService.Domain.Enums.EstadoRuta>(estado, true);
        await _db.SaveChangesAsync();
        return route;
    }
}

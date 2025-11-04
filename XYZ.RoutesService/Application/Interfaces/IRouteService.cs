using XYZ.RoutesService.Domain.Entities;
using Route = XYZ.RoutesService.Domain.Entities.Route;

namespace XYZ.RoutesService.Application.Interfaces;

public interface IRouteService
{
    Task<Route> CreateAsync(string nombre, string origen, string destino, string vehiclePlaca, int driverId);
    Task<Route?> GetByIdAsync(int id);
    Task<List<Route>> GetAllAsync();
    Task<Route?> UpdateStatusAsync(int id, string estado);
}

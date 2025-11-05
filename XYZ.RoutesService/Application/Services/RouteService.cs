using System.Net.Http.Json;
using XYZ.RoutesService.Application.Interfaces;
using XYZ.RoutesService.Domain.Entities;
using Route = XYZ.RoutesService.Domain.Entities.Route;
using XYZ.RoutesService.Infrastructure.Repositories;

namespace XYZ.RoutesService.Application.Services;

public class RouteService : IRouteService
{
    private readonly RouteRepository _repo;
    private readonly GeocodingService _geocoding;
    private readonly HttpClient _httpClient;

    public RouteService(RouteRepository repo, GeocodingService geocoding, IHttpClientFactory factory)
    {
        _repo = repo;
        _geocoding = geocoding;
        _httpClient = factory.CreateClient("GeoClient");
    }

    public async Task<Route> CreateAsync(string nombre, string origen, string destino, string vehiclePlaca, int driverId)
    {
        // Validaciones básicas de entrada
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la ruta no puede estar vacío.", nameof(nombre));
        if (string.IsNullOrWhiteSpace(origen))
            throw new ArgumentException("El origen no puede estar vacío.", nameof(origen));
        if (string.IsNullOrWhiteSpace(destino))
            throw new ArgumentException("El destino no puede estar vacío.", nameof(destino));
        if (string.IsNullOrWhiteSpace(vehiclePlaca))
            throw new ArgumentException("vehiclePlaca no puede estar vacío.", nameof(vehiclePlaca));
        if (driverId <= 0)
            throw new ArgumentException("driverId debe ser un entero positivo.", nameof(driverId));

        // Validar que las direcciones resuelvan a coordenadas
        (double lat, double lon) origenCoord;
        (double lat, double lon) destinoCoord;
        try
        {
            origenCoord = await _geocoding.GetCoordinatesAsync(origen);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"No se pudo geocodificar el origen: {ex.Message}", ex);
        }

        try
        {
            destinoCoord = await _geocoding.GetCoordinatesAsync(destino);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"No se pudo geocodificar el destino: {ex.Message}", ex);
        }

        var (distanciaKm, duracionMinutos) = await CalcularDistanciaYTiempoAsync(origenCoord, destinoCoord);

        if (distanciaKm <= 0 || duracionMinutos <= 0)
            throw new InvalidOperationException("No se pudo calcular una ruta válida entre el origen y el destino proporcionados.");

        var route = new Route
        {
            Nombre = nombre,
            Origen = origen,
            Destino = destino,
            DistanciaKm = distanciaKm,
            DuracionMinutos = duracionMinutos,
            VehiclePlaca = vehiclePlaca,
            DriverId = driverId
        };

        return await _repo.AddAsync(route);
    }

    public Task<Route?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<List<Route>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Route?> UpdateStatusAsync(int id, string estado) => _repo.UpdateStatusAsync(id, estado);

    public async Task<Route?> UpdateAsync(int id, string nombre, string origen, string destino, string vehiclePlaca, int driverId)
    {
        // Basic validation
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la ruta no puede estar vacío.", nameof(nombre));
        if (string.IsNullOrWhiteSpace(origen))
            throw new ArgumentException("El origen no puede estar vacío.", nameof(origen));
        if (string.IsNullOrWhiteSpace(destino))
            throw new ArgumentException("El destino no puede estar vacío.", nameof(destino));
        if (string.IsNullOrWhiteSpace(vehiclePlaca))
            throw new ArgumentException("vehiclePlaca no puede estar vacío.", nameof(vehiclePlaca));
        if (driverId <= 0)
            throw new ArgumentException("driverId debe ser un entero positivo.", nameof(driverId));

        // Geocode origen/destino
        (double lat, double lon) origenCoord;
        (double lat, double lon) destinoCoord;
        try
        {
            origenCoord = await _geocoding.GetCoordinatesAsync(origen);
            destinoCoord = await _geocoding.GetCoordinatesAsync(destino);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"No se pudo geocodificar una de las direcciones: {ex.Message}", ex);
        }

        var (distanciaKm, duracionMinutos) = await CalcularDistanciaYTiempoAsync(origenCoord, destinoCoord);

        if (distanciaKm <= 0 || duracionMinutos <= 0)
            throw new InvalidOperationException("No se pudo calcular una ruta válida entre el origen y el destino proporcionados.");

        var route = new Route
        {
            Id = id,
            Nombre = nombre,
            Origen = origen,
            Destino = destino,
            DistanciaKm = distanciaKm,
            DuracionMinutos = duracionMinutos,
            VehiclePlaca = vehiclePlaca,
            DriverId = driverId
        };

        return await _repo.UpdateAsync(route);
    }

    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

    private async Task<(double distanciaKm, double duracionMinutos)> CalcularDistanciaYTiempoAsync(
        (double lat, double lon) origen, (double lat, double lon) destino)
    {
        var url = $"http://router.project-osrm.org/route/v1/driving/{origen.lon},{origen.lat};{destino.lon},{destino.lat}?overview=false";
        var response = await _httpClient.GetFromJsonAsync<OsrmResponse>(url);

        var route = response?.routes?.FirstOrDefault();
        if (route == null)
            return (0, 0);

        double distanciaKm = route.distance / 1000; // metros → km
        double duracionMinutos = route.duration / 60; // segundos → minutos

        return (distanciaKm, duracionMinutos);
    }

    private class OsrmResponse
    {
        public List<RouteData>? routes { get; set; }
    }

    private class RouteData
    {
        public double distance { get; set; }
        public double duration { get; set; }
    }
}

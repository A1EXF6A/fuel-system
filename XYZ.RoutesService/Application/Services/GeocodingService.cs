using System.Net.Http.Json;

namespace XYZ.RoutesService.Application.Services;

public class GeocodingService
{
    private readonly HttpClient _httpClient;

    public GeocodingService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("GeoClient");
    }

    public async Task<(double lat, double lon)> GetCoordinatesAsync(string address)
    {
        var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&limit=1";
        var response = await _httpClient.GetFromJsonAsync<List<GeoResult>>(url);

        if (response == null || response.Count == 0)
            throw new Exception($"No se encontró coordenada para: {address}");

        var result = response.First();
        return (result.lat, result.lon);
    }

    private class GeoResult
    {
        public double lat { get; set; }
        public double lon { get; set; }
    }
}

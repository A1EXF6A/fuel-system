using Grpc.Net.Client;
using XYZ.FuelService.Application.Interfaces;
using XYZ.FuelService.Domain.Entities;
using XYZ.FuelService.Domain.Enums;
using XYZ.FuelService.Infrastructure.Repositories;
using XYZ.RoutesService.Protos;
using XYZ.VehiclesService.Protos;

namespace XYZ.FuelService.Application.Services;

public class FuelService : IFuelService
{
    private readonly FuelRepository _repo;
    private readonly HttpClient _http;

    private readonly Vehicles.VehiclesClient _vehiclesClient;
    private readonly Routes.RoutesClient _routesClient;

    public FuelService(
        IConfiguration configuration,
        FuelRepository repo,
        IHttpClientFactory factory
    )
    {
        string vehiclesServiceUrl =
            configuration.GetValue<string>("Services:VehiclesService")
            ?? "http://vehiclesservice:5002";

        var channelVehicles = GrpcChannel.ForAddress(vehiclesServiceUrl);
        _vehiclesClient = new Vehicles.VehiclesClient(channelVehicles);

        string routesServiceUrl =
            configuration.GetValue<string>("Services:RoutesService") ?? "http://routesservice:5004";

        var channelRoutes = GrpcChannel.ForAddress(routesServiceUrl);
        _routesClient = new Routes.RoutesClient(channelRoutes);

        _repo = repo;
        _http = factory.CreateClient();
    }

    public async Task<FuelRecord> CreateFuelPlanAsync(
        string? vehiclePlaca,
        int driverId,
        int routeId
    )
    {
        double distanciaKm = 0.0;

        if (routeId > 0)
        {
            var rreq = new GetRouteRequest { Id = routeId };
            var rresp =
                await _routesClient.GetRouteByIdAsync(rreq)
                ?? throw new Exception("Route not found");

            // if vehiclePlaca was not provided by caller, take it from the route
            if (string.IsNullOrWhiteSpace(vehiclePlaca))
                vehiclePlaca = rresp.VehiclePlaca;

            distanciaKm = rresp.DistanciaKm;
            // set driverId to route driver if caller didn't provide a driverId
            if (driverId == 0)
                driverId = rresp.DriverId;
        }

        if (string.IsNullOrWhiteSpace(vehiclePlaca))
            throw new Exception("Vehicle placa is required either directly or via route");

        // 🔹 1b. Obtener datos de Vehicle via gRPC (GetVehicleByPlaca)
        var getReq = new GetVehicleByPlacaRequest { Placa = vehiclePlaca };
        var vresp =
            await _vehiclesClient.GetVehicleByPlacaAsync(getReq)
            ?? throw new Exception("Vehicle not found");

        // 🔹 2. Obtener tipos para factores desde Vehicles (vehicle types)
        var typesResp = await _vehiclesClient.GetVehicleTypesAsync(
            new VehiclesService.Protos.EmptyRequest()
        );
        var matchedType = typesResp.Types_.FirstOrDefault(t =>
            t.TipoMaquinaria == vresp.TipoMaquinaria
        );

        double factorMotor = matchedType?.FactorMotor ?? 1.0;
        double pavimentado = matchedType?.Pavimentado ?? 1.0;
        // routes currently do not expose tipoTerreno; default to pavimentado factor
        double factorTerreno = pavimentado;

        // if distancia wasn't obtained from route, try to fetch basic route info via HTTP fallback
        if (distanciaKm == 0.0 && routeId > 0)
        {
            // fallback to previous HTTP call (kept for compatibility)
            var route = await _http.GetFromJsonAsync<RouteResponse>(
                $"http://routesservice:5003/routes/{routeId}"
            );
            if (route != null)
                distanciaKm = route.distanciaKm;
        }

        double distanceToUse = distanciaKm > 0 ? distanciaKm : 0.0;
        double estimatedLiters =
            (distanceToUse / 100) * vresp.ConsumoBase * factorMotor * factorTerreno;

        var record = new FuelRecord
        {
            VehicleId = vresp.Id,
            VehiclePlaca = vehiclePlaca,
            DriverId = driverId,
            RouteId = routeId,
            TipoMaquinaria = vresp.TipoMaquinaria,
            DistanceKm = distanceToUse,
            EstimatedLiters = estimatedLiters,
            Estado = EstadoConsumo.Planificado,
        };

        return await _repo.AddAsync(record);
    }

    public async Task<FuelRecord?> RegisterActualConsumptionAsync(int planId, double actualLiters)
    {
        var record = await _repo.GetByIdAsync(planId);
        if (record == null)
            return null;

        record.ActualLiters = actualLiters;
        record.Estado = EstadoConsumo.Completado;
        await _repo.UpdateAsync(record);
        return record;
    }

    public Task<List<FuelRecord>> GetFuelReportAsync(string filterType, string filterValue) =>
        _repo.GetByFilterAsync(filterType, filterValue);

    public Task<List<FuelRecord>> GetAllReportsAsync() => _repo.GetAllAsync();

    public Task<FuelRecord?> UpdateReportStatusAsync(int id, string estado) =>
        _repo.UpdateStatusAsync(id, estado);

    // 🔸 Clases auxiliares para deserialización HTTP
    private class VehicleResponse
    {
        public string tipoMaquinaria { get; set; } = string.Empty;
        public double consumoBase { get; set; }
        public double factorMotor { get; set; }
        public double pavimentado { get; set; }
        public double montañoso { get; set; }
        public double mixto { get; set; }
    }

    private class RouteResponse
    {
        public double distanciaKm { get; set; }
        public string tipoTerreno { get; set; } = "pavimentado";
    }
}

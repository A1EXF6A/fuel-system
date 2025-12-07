using Grpc.Core;
using Grpc.Net.Client;
using XYZ.DriversService.Protos;
using XYZ.RoutesService.Application.Interfaces;
using XYZ.RoutesService.Protos;
using XYZ.VehiclesService.Protos;

namespace XYZ.RoutesService.Controllers;

public class RoutesGrpcService : Routes.RoutesBase
{
    private readonly IRouteService _service;
    private readonly Drivers.DriversClient _driversClient;
    private readonly Vehicles.VehiclesClient _vehiclesClient;

    public RoutesGrpcService(IConfiguration configuration, IRouteService service)
    {
        string driversServiceURL =
            configuration.GetValue<string>("Services:DriversService") ?? "http://localhost:5001";

        var driversChannel = GrpcChannel.ForAddress(driversServiceURL);

        _driversClient = new Drivers.DriversClient(driversChannel);

        string vehiclesServiceURL =
            configuration.GetValue<string>("Services:VehiclesService") ?? "http://localhost:5002";

        var vehiclesChannel = GrpcChannel.ForAddress(vehiclesServiceURL);
        _vehiclesClient = new Vehicles.VehiclesClient(vehiclesChannel);

        _service = service;
    }

    public override async Task<RouteResponse> CreateRoute(
        CreateRouteRequest request,
        ServerCallContext context
    )
    {
        try
        {
            // Validate driver exists and is assigned to the provided vehicle placa
            GetDriverResponse response;
            try
            {
                response = await _driversClient.GetDriverAsync(
                    new GetDriverRequest { Id = request.DriverId }
                );
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                throw new InvalidOperationException("Driver not found");
            }

            if (
                !response.Driver.IsAssigned
                || response.Driver.AssignedVehiclePlaca != request.VehiclePlaca
            )
                throw new InvalidOperationException(
                    "Driver is not assigned to the specified vehicle"
                );

            try
            {
                await _vehiclesClient.GetVehicleByPlacaAsync(
                    new GetVehicleByPlacaRequest { Placa = request.VehiclePlaca }
                );
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                throw new InvalidOperationException("Vehicle not found");
            }

            var route = await _service.CreateAsync(
                request.Nombre,
                request.Origen,
                request.Destino,
                request.VehiclePlaca,
                request.DriverId
            );

            return new RouteResponse
            {
                Id = route.Id,
                Nombre = route.Nombre,
                Origen = route.Origen,
                Destino = route.Destino,
                DistanciaKm = route.DistanciaKm,
                DuracionMinutos = route.DuracionMinutos,
                Estado = route.Estado.ToString(),
                VehiclePlaca = route.VehiclePlaca,
                DriverId = route.DriverId,
            };
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            // Could be geocoding/OSRM problems or invalid route
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (Exception ex)
        {
            // Generic server error
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<RouteResponse> GetRouteById(
        GetRouteRequest request,
        ServerCallContext context
    )
    {
        var route = await _service.GetByIdAsync(request.Id);
        return route == null
            ? throw new RpcException(new Status(StatusCode.NotFound, "Route not found"))
            : new RouteResponse
            {
                Id = route.Id,
                Nombre = route.Nombre,
                Origen = route.Origen,
                Destino = route.Destino,
                DistanciaKm = route.DistanciaKm,
                DuracionMinutos = route.DuracionMinutos,
                Estado = route.Estado.ToString(),
                VehiclePlaca = route.VehiclePlaca,
                DriverId = route.DriverId,
            };
    }

    public override async Task<RoutesListResponse> GetAllRoutes(
        Protos.EmptyRequest request,
        ServerCallContext context
    )
    {
        var list = await _service.GetAllAsync();
        var response = new RoutesListResponse();
        response.Routes.AddRange(
            list.Select(r => new RouteResponse
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Origen = r.Origen,
                Destino = r.Destino,
                DistanciaKm = r.DistanciaKm,
                DuracionMinutos = r.DuracionMinutos,
                Estado = r.Estado.ToString(),
                VehiclePlaca = r.VehiclePlaca,
                DriverId = r.DriverId,
            })
        );
        return response;
    }

    public override async Task<RouteResponse> UpdateRouteStatus(
        UpdateRouteStatusRequest request,
        ServerCallContext context
    )
    {
        var route = await _service.UpdateStatusAsync(request.Id, request.Estado);
        return route == null
            ? throw new RpcException(new Status(StatusCode.NotFound, "Route not found"))
            : new RouteResponse { Id = route.Id, Estado = route.Estado.ToString() };
    }

    public override async Task<RouteResponse> UpdateRoute(
        UpdateRouteRequest request,
        ServerCallContext context
    )
    {
        try
        {
            // Validate driver exists and is assigned to the provided vehicle placa
            var driversUrl =
                Environment.GetEnvironmentVariable("DRIVERS_SERVICE_URL")
                ?? "http://driversservice:80";
            using var chDrivers = GrpcChannel.ForAddress(driversUrl);
            var driversClient = new Drivers.DriversClient(chDrivers);
            GetDriverResponse drvResp;
            try
            {
                drvResp = await driversClient.GetDriverAsync(
                    new GetDriverRequest { Id = request.DriverId }
                );
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                throw new InvalidOperationException("Driver not found");
            }

            if (
                !drvResp.Driver.IsAssigned
                || drvResp.Driver.AssignedVehiclePlaca != request.VehiclePlaca
            )
                throw new InvalidOperationException(
                    "Driver is not assigned to the specified vehicle"
                );

            // Validate vehicle exists
            var vehiclesUrl =
                Environment.GetEnvironmentVariable("VEHICLES_SERVICE_URL")
                ?? "http://vehiclesservice:5002";
            using var chVehicles = GrpcChannel.ForAddress(vehiclesUrl);
            var vehiclesClient = new Vehicles.VehiclesClient(chVehicles);
            try
            {
                await vehiclesClient.GetVehicleByPlacaAsync(
                    new GetVehicleByPlacaRequest { Placa = request.VehiclePlaca }
                );
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                throw new InvalidOperationException("Vehicle not found");
            }

            var route = await _service.UpdateAsync(
                request.Id,
                request.Nombre,
                request.Origen,
                request.Destino,
                request.VehiclePlaca,
                request.DriverId
            );
            return route == null
                ? throw new RpcException(new Status(StatusCode.NotFound, "Route not found"))
                : new RouteResponse
                {
                    Id = route.Id,
                    Nombre = route.Nombre,
                    Origen = route.Origen,
                    Destino = route.Destino,
                    DistanciaKm = route.DistanciaKm,
                    DuracionMinutos = route.DuracionMinutos,
                    Estado = route.Estado.ToString(),
                    VehiclePlaca = route.VehiclePlaca,
                    DriverId = route.DriverId,
                };
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<DeleteRouteResponse> DeleteRoute(
        DeleteRouteRequest request,
        ServerCallContext context
    )
    {
        var ok = await _service.DeleteAsync(request.Id);
        return new DeleteRouteResponse { Success = ok };
    }
}

using Grpc.Core;
using XYZ.RoutesService.Application.Interfaces;
using XYZ.RoutesService.Protos;
using Route = XYZ.RoutesService.Domain.Entities.Route;

namespace XYZ.RoutesService.Controllers;

public class RoutesGrpcService : Routes.RoutesBase
{
    private readonly IRouteService _service;

    public RoutesGrpcService(IRouteService service)
    {
        _service = service;
    }

    public override async Task<RouteResponse> CreateRoute(CreateRouteRequest request, ServerCallContext context)
    {
        try
        {
            // Validate driver exists and is assigned to the provided vehicle placa
            var driversUrl = Environment.GetEnvironmentVariable("DRIVERS_SERVICE_URL") ?? "http://driversservice:80";
            using var chDrivers = Grpc.Net.Client.GrpcChannel.ForAddress(driversUrl);
            var driversClient = new XYZ.DriversService.Protos.Drivers.DriversClient(chDrivers);
            XYZ.DriversService.Protos.GetDriverResponse drvResp;
            try
            {
                drvResp = await driversClient.GetDriverAsync(new XYZ.DriversService.Protos.GetDriverRequest { Id = request.DriverId });
            }
            catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                throw new InvalidOperationException("Driver not found");
            }

            if (!drvResp.Driver.IsAssigned || drvResp.Driver.AssignedVehiclePlaca != request.VehiclePlaca)
                throw new InvalidOperationException("Driver is not assigned to the specified vehicle");

            // Validate vehicle exists
            var vehiclesUrl = Environment.GetEnvironmentVariable("VEHICLES_SERVICE_URL") ?? "http://vehiclesservice:5002";
            using var chVehicles = Grpc.Net.Client.GrpcChannel.ForAddress(vehiclesUrl);
            var vehiclesClient = new XYZ.VehiclesService.Protos.Vehicles.VehiclesClient(chVehicles);
            try
            {
                await vehiclesClient.GetVehicleByPlacaAsync(new XYZ.VehiclesService.Protos.GetVehicleByPlacaRequest { Placa = request.VehiclePlaca });
            }
            catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                throw new InvalidOperationException("Vehicle not found");
            }

            var route = await _service.CreateAsync(request.Nombre, request.Origen, request.Destino, request.VehiclePlaca, request.DriverId);

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
                DriverId = route.DriverId
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

    public override async Task<RouteResponse> GetRouteById(GetRouteRequest request, ServerCallContext context)
    {
        var route = await _service.GetByIdAsync(request.Id);
        if (route == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Route not found"));

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
            DriverId = route.DriverId
        };
    }

    public override async Task<RoutesListResponse> GetAllRoutes(EmptyRequest request, ServerCallContext context)
    {
        var list = await _service.GetAllAsync();
        var response = new RoutesListResponse();
            response.Routes.AddRange(list.Select(r => new RouteResponse
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Origen = r.Origen,
            Destino = r.Destino,
            DistanciaKm = r.DistanciaKm,
            DuracionMinutos = r.DuracionMinutos,
            Estado = r.Estado.ToString(),
                VehiclePlaca = r.VehiclePlaca,
            DriverId = r.DriverId
        }));
        return response;
    }

    public override async Task<RouteResponse> UpdateRouteStatus(UpdateRouteStatusRequest request, ServerCallContext context)
    {
        var route = await _service.UpdateStatusAsync(request.Id, request.Estado);
        if (route == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Route not found"));

        return new RouteResponse
        {
            Id = route.Id,
            Estado = route.Estado.ToString()
        };
    }
}

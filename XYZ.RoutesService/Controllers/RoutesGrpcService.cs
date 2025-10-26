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
            var route = await _service.CreateAsync(request.Nombre, request.Origen, request.Destino, request.VehicleId, request.DriverId);

            return new RouteResponse
            {
                Id = route.Id,
                Nombre = route.Nombre,
                Origen = route.Origen,
                Destino = route.Destino,
                DistanciaKm = route.DistanciaKm,
                DuracionMinutos = route.DuracionMinutos,
                Estado = route.Estado.ToString(),
                VehicleId = route.VehicleId,
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
            VehicleId = route.VehicleId,
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
            VehicleId = r.VehicleId,
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

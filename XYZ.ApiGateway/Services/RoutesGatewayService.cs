using Grpc.Core;
using Grpc.Net.Client;
using XYZ.RoutesService.Protos;

namespace XYZ.ApiGateway.Services;

public class RoutesGatewayService
{
    private readonly Routes.RoutesClient _routesClient;
    private readonly ILogger<RoutesGatewayService> _logger;

    public RoutesGatewayService(IConfiguration configuration, ILogger<RoutesGatewayService> logger)
    {
        var routesServiceUrl = configuration.GetValue<string>("Services:RoutesService");
        var channel = GrpcChannel.ForAddress(routesServiceUrl!);
        _routesClient = new Routes.RoutesClient(channel);
        _logger = logger;
    }

    public async Task<RouteResponse> CreateRouteAsync(CreateRouteRequest request)
    {
        try
        {
            var response = await _routesClient.CreateRouteAsync(request);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error creating route");
            throw;
        }
    }

    public async Task<RouteResponse> GetRouteByIdAsync(int id)
    {
        try
        {
            var request = new GetRouteRequest { Id = id };
            var response = await _routesClient.GetRouteByIdAsync(request);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error getting route by id {Id}", id);
            throw;
        }
    }

    public async Task<RoutesListResponse> GetAllRoutesAsync()
    {
        try
        {
            var request = new EmptyRequest();
            var response = await _routesClient.GetAllRoutesAsync(request);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error getting all routes");
            throw;
        }
    }
}

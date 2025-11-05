using Grpc.Core;
using Grpc.Net.Client;
using XYZ.RoutesService.Protos;

namespace XYZ.ApiGateway.Services;

public class RoutesGatewayService
{
    private readonly Routes.RoutesClient _routesClient;
    private readonly ILogger<RoutesGatewayService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RoutesGatewayService(IConfiguration configuration, ILogger<RoutesGatewayService> logger, IHttpContextAccessor httpContextAccessor)
    {
        var routesServiceUrl = configuration.GetValue<string>("Services:RoutesService");
        var channel = GrpcChannel.ForAddress(routesServiceUrl!);
        _routesClient = new Routes.RoutesClient(channel);
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<RouteResponse> CreateRouteAsync(CreateRouteRequest request)
    {
        try
        {
            var metadata = BuildAuthMetadata();
            var response = await _routesClient.CreateRouteAsync(request, metadata);
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
            var metadata = BuildAuthMetadata();
            var response = await _routesClient.GetRouteByIdAsync(request, metadata);
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
            var metadata = BuildAuthMetadata();
            var response = await _routesClient.GetAllRoutesAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error getting all routes");
            throw;
        }
    }

    public async Task<RouteResponse> UpdateRouteAsync(UpdateRouteRequest request)
    {
        try
        {
            var metadata = BuildAuthMetadata();
            var response = await _routesClient.UpdateRouteAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error updating route with ID: {Id}", request.Id);
            throw;
        }
    }

    public async Task<DeleteRouteResponse> DeleteRouteAsync(int id)
    {
        try
        {
            var request = new DeleteRouteRequest { Id = id };
            var metadata = BuildAuthMetadata();
            var response = await _routesClient.DeleteRouteAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error deleting route with ID: {Id}", id);
            throw;
        }
    }

    public async Task<RouteResponse> UpdateRouteStatusAsync(UpdateRouteStatusRequest request)
    {
        try
        {
            var metadata = BuildAuthMetadata();
            var response = await _routesClient.UpdateRouteStatusAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error updating route status for ID: {Id}", request.Id);
            throw;
        }
    }

    private Metadata? BuildAuthMetadata()
    {
        try
        {
            var auth = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(auth))
            {
                var metadata = new Metadata();
                metadata.Add("Authorization", auth);
                return metadata;
            }
        }
        catch { }
        return null;
    }
}

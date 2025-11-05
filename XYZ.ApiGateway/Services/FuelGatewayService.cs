using Grpc.Core;
using Grpc.Net.Client;
using XYZ.FuelService.Protos;

namespace XYZ.ApiGateway.Services;

public class FuelGatewayService
{
    private readonly Fuel.FuelClient _fuelClient;
    private readonly ILogger<FuelGatewayService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FuelGatewayService(IConfiguration configuration, ILogger<FuelGatewayService> logger, IHttpContextAccessor httpContextAccessor)
    {
        var fuelServiceUrl = configuration.GetValue<string>("Services:FuelService");
        var channel = GrpcChannel.ForAddress(fuelServiceUrl!);
        _fuelClient = new Fuel.FuelClient(channel);
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<FuelPlanResponse> CreateFuelPlanAsync(string vehiclePlaca, int driverId, int routeId)
    {
        try
        {
            var request = new FuelPlanRequest { VehiclePlaca = vehiclePlaca, DriverId = driverId, RouteId = routeId };
            var metadata = BuildAuthMetadata();
            var response = await _fuelClient.CreateFuelPlanAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error creating fuel plan");
            throw;
        }
    }

    public async Task<FuelPlanResponse> RegisterActualConsumptionAsync(int planId, double actualLiters)
    {
        try
        {
            var request = new ActualConsumptionRequest { PlanId = planId, ActualLiters = actualLiters };
            var metadata = BuildAuthMetadata();
            var response = await _fuelClient.RegisterActualConsumptionAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error registering actual consumption");
            throw;
        }
    }

    public async Task<FuelReportResponse> GetFuelReportAsync(string filterType, string filterValue)
    {
        try
        {
            var request = new FuelReportRequest { FilterType = filterType, FilterValue = filterValue };
            var metadata = BuildAuthMetadata();
            var response = await _fuelClient.GetFuelReportAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error getting fuel report");
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

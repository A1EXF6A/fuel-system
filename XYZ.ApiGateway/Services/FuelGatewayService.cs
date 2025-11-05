using Grpc.Core;
using Grpc.Net.Client;
using XYZ.FuelService.Protos;

namespace XYZ.ApiGateway.Services;

public class FuelGatewayService
{
    private readonly Fuel.FuelClient _fuelClient;
    private readonly ILogger<FuelGatewayService> _logger;

    public FuelGatewayService(IConfiguration configuration, ILogger<FuelGatewayService> logger)
    {
        var fuelServiceUrl = configuration.GetValue<string>("Services:FuelService");
        var channel = GrpcChannel.ForAddress(fuelServiceUrl!);
        _fuelClient = new Fuel.FuelClient(channel);
        _logger = logger;
    }

    public async Task<FuelPlanResponse> CreateFuelPlanAsync(string vehiclePlaca, int driverId, int routeId)
    {
        try
        {
            var request = new FuelPlanRequest { VehiclePlaca = vehiclePlaca, DriverId = driverId, RouteId = routeId };
            var response = await _fuelClient.CreateFuelPlanAsync(request);
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
            var response = await _fuelClient.RegisterActualConsumptionAsync(request);
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
            var response = await _fuelClient.GetFuelReportAsync(request);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error getting fuel report");
            throw;
        }
    }
}

using Grpc.Core;
using Grpc.Net.Client;
using XYZ.VehiclesService.Protos;

namespace XYZ.ApiGateway.Services;

public class VehiclesGatewayService
{
    private readonly Vehicles.VehiclesClient _vehiclesClient;
    private readonly ILogger<VehiclesGatewayService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public VehiclesGatewayService(IConfiguration configuration, ILogger<VehiclesGatewayService> logger, IHttpContextAccessor httpContextAccessor)
    {
        var vehiclesServiceUrl = configuration.GetValue<string>("Services:VehiclesService");
        var channel = GrpcChannel.ForAddress(vehiclesServiceUrl!);
        _vehiclesClient = new Vehicles.VehiclesClient(channel);
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<VehicleResponse> GetVehicleByPlacaAsync(string placa)
    {
        try
        {
            var request = new GetVehicleByPlacaRequest { Placa = placa };
            var response = await _vehiclesClient.GetVehicleByPlacaAsync(request);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error getting vehicle by placa: {Placa}", placa);
            throw;
        }
    }

    public async Task<VehicleResponse> CreateVehicleAsync(CreateVehicleRequest request)
    {
        try
        {
            var response = await _vehiclesClient.CreateVehicleAsync(request);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error creating vehicle");
            throw;
        }
    }

    public async Task<VehiclesListResponse> GetAllVehiclesAsync()
    {
        try
        {
            var request = new EmptyRequest();
            var metadata = BuildAuthMetadata();
            var response = await _vehiclesClient.GetAllVehiclesAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error getting all vehicles");
            throw;
        }
    }

    public async Task<VehicleResponse> SetAssignedDriverAsync(string placa, string driverDocument)
    {
        try
        {
            var request = new SetAssignedDriverRequest { Placa = placa, DriverDocument = driverDocument };
            var metadata = BuildAuthMetadata();
            var response = await _vehiclesClient.SetAssignedDriverAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error setting assigned driver for placa {Placa}", placa);
            throw;
        }
    }

    public async Task<VehicleResponse> GetVehicleByIdAsync(int id)
    {
        try
        {
            var request = new GetVehicleRequest { Id = id };
            var metadata = BuildAuthMetadata();
            var response = await _vehiclesClient.GetVehicleByIdAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error getting vehicle by id {Id}", id);
            throw;
        }
    }

    public async Task<VehicleTypesListResponse> GetVehicleTypesAsync()
    {
        try
        {
            var request = new EmptyRequest();
            var metadata = BuildAuthMetadata();
            var response = await _vehiclesClient.GetVehicleTypesAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error getting vehicle types");
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

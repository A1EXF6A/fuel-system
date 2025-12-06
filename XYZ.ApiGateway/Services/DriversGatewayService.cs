using Grpc.Core;
using Grpc.Net.Client;
using XYZ.DriversService.Protos;
using Google.Protobuf.WellKnownTypes;

namespace XYZ.ApiGateway.Services;

public class DriversGatewayService
{
    private readonly Drivers.DriversClient _driversClient;
    private readonly ILogger<DriversGatewayService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DriversGatewayService(IConfiguration configuration, ILogger<DriversGatewayService> logger, IHttpContextAccessor httpContextAccessor)
    {
        var driversServiceUrl = configuration.GetValue<string>("Services:DriversService");
        var channel = GrpcChannel.ForAddress(driversServiceUrl!);
        _driversClient = new Drivers.DriversClient(channel);
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<GetAllDriversResponse> GetAllDriversAsync()
    {
        try
        {
            var request = new GetAllDriversRequest();
            var metadata = BuildAuthMetadata();
            var response = await _driversClient.GetAllDriversAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error getting all drivers");
            throw new Exception($"Failed to get drivers: {ex.Status.Detail}");
        }
    }

    public async Task<GetDriverResponse> GetDriverAsync(int id)
    {
        try
        {
            var request = new GetDriverRequest { Id = id };
            var metadata = BuildAuthMetadata();
            var response = await _driversClient.GetDriverAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error getting driver with ID: {DriverId}", id);
            throw new Exception($"Failed to get driver: {ex.Status.Detail}");
        }
    }

    public async Task<CreateDriverResponse> CreateDriverAsync(
        string firstName, string lastName, string documentNumber, string phoneNumber,
        string email, string licenseNumber, int licenseCategory, DateTime licenseExpiryDate,
        int driverType, DateTime hireDate)
    {
        try
        {
            var request = new CreateDriverRequest
            {
                FirstName = firstName,
                LastName = lastName,
                DocumentNumber = documentNumber,
                PhoneNumber = phoneNumber,
                Email = email,
                LicenseNumber = licenseNumber,
                LicenseCategory = licenseCategory,
                LicenseExpiryDate = Timestamp.FromDateTime(licenseExpiryDate.ToUniversalTime()),
                DriverType = driverType,
                HireDate = Timestamp.FromDateTime(hireDate.ToUniversalTime())
            };

            var metadata = BuildAuthMetadata();
            var response = await _driversClient.CreateDriverAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error creating driver: {FirstName} {LastName}", firstName, lastName);
            throw new Exception($"Failed to create driver: {ex.Status.Detail}");
        }
    }

    public async Task<UpdateDriverResponse> UpdateDriverAsync(
        int id, string firstName, string lastName, string phoneNumber, string email,
        string licenseNumber, int licenseCategory, DateTime licenseExpiryDate,
        int driverType, int status)
    {
        try
        {
            var request = new UpdateDriverRequest
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Email = email,
                LicenseNumber = licenseNumber,
                LicenseCategory = licenseCategory,
                LicenseExpiryDate = Timestamp.FromDateTime(licenseExpiryDate.ToUniversalTime()),
                DriverType = driverType,
                Status = status
            };

            var metadata = BuildAuthMetadata();
            var response = await _driversClient.UpdateDriverAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error updating driver with ID: {DriverId}", id);
            throw new Exception($"Failed to update driver: {ex.Status.Detail}");
        }
    }

    public async Task<GetAvailableDriversResponse> GetAvailableDriversAsync()
    {
        try
        {
            var request = new GetAvailableDriversRequest();
            var metadata = BuildAuthMetadata();
            var response = await _driversClient.GetAvailableDriversAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error getting available drivers");
            throw new Exception($"Failed to get available drivers: {ex.Status.Detail}");
        }
    }

    public async Task<AssignDriverResponse> AssignDriverAsync(int driverId, string vehiclePlaca)
    {
        try
        {
            var request = new AssignDriverRequest
            {
                DriverId = driverId,
                VehiclePlaca = vehiclePlaca
            };

            var metadata = BuildAuthMetadata();
            var response = await _driversClient.AssignDriverAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error assigning driver {DriverId} to vehicle placa {VehiclePlaca}", driverId, vehiclePlaca);
            throw new Exception($"Failed to assign driver: {ex.Status.Detail}");
        }
    }

    public async Task<UnassignDriverResponse> UnassignDriverAsync(int driverId)
    {
        try
        {
            var request = new UnassignDriverRequest
            {
                DriverId = driverId
            };

            var metadata = BuildAuthMetadata();
            var response = await _driversClient.UnassignDriverAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error unassigning driver {DriverId}", driverId);
            throw new Exception($"Failed to unassign driver: {ex.Status.Detail}");
        }
    }

    public async Task<DeleteDriverResponse> DeleteDriverAsync(int id, string deletedBy, string? reason = null)
    {
        try
        {
            var request = new DeleteDriverRequest { Id = id };
            var metadata = BuildAuthMetadata();
            var response = await _driversClient.DeleteDriverAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error deleting driver with ID: {DriverId}", id);
            throw new Exception($"Failed to delete driver: {ex.Status.Detail}");
        }
    }



    public async Task<GetDriverResponse> GetDriverByDocumentNumberAsync(string documentNumber)
    {
        try
        {
            var request = new GetDriverByDocumentNumberRequest { DocumentNumber = documentNumber };
            var metadata = BuildAuthMetadata();
            var response = await _driversClient.GetDriverByDocumentNumberAsync(request, metadata);
            return response;
        }
        catch (RpcException ex)
        {
            // If the RPC is not implemented, return null-like behavior via exception
            _logger.LogError(ex, "Error getting driver by document number: {DocumentNumber}", documentNumber);
            throw new Exception($"Failed to get driver by document number: {ex.Status.Detail}");
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

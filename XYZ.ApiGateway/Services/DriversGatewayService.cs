using Grpc.Core;
using Grpc.Net.Client;
using XYZ.DriversService.Protos;
using Google.Protobuf.WellKnownTypes;

namespace XYZ.ApiGateway.Services;

public class DriversGatewayService
{
    private readonly Drivers.DriversClient _driversClient;
    private readonly ILogger<DriversGatewayService> _logger;

    public DriversGatewayService(IConfiguration configuration, ILogger<DriversGatewayService> logger)
    {
        var driversServiceUrl = configuration.GetValue<string>("Services:DriversService");
        var channel = GrpcChannel.ForAddress(driversServiceUrl!);
        _driversClient = new Drivers.DriversClient(channel);
        _logger = logger;
    }

    public async Task<GetAllDriversResponse> GetAllDriversAsync()
    {
        try
        {
            var request = new GetAllDriversRequest();
            var response = await _driversClient.GetAllDriversAsync(request);
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
            var response = await _driversClient.GetDriverAsync(request);
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

            var response = await _driversClient.CreateDriverAsync(request);
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

            var response = await _driversClient.UpdateDriverAsync(request);
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
            var response = await _driversClient.GetAvailableDriversAsync(request);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error getting available drivers");
            throw new Exception($"Failed to get available drivers: {ex.Status.Detail}");
        }
    }

    public async Task<AssignDriverResponse> AssignDriverAsync(int driverId, string vehicleId)
    {
        try
        {
            var request = new AssignDriverRequest
            {
                DriverId = driverId,
                VehicleId = vehicleId
            };

            var response = await _driversClient.AssignDriverAsync(request);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error assigning driver {DriverId} to vehicle {VehicleId}", driverId, vehicleId);
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

            var response = await _driversClient.UnassignDriverAsync(request);
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
            var request = new DeleteDriverRequest 
            { 
                Id = id,
                DeletedBy = deletedBy,
                Reason = reason ?? string.Empty
            };
            var response = await _driversClient.DeleteDriverAsync(request);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error deleting driver with ID: {DriverId}", id);
            throw new Exception($"Failed to delete driver: {ex.Status.Detail}");
        }
    }

    public async Task<RestoreDriverResponse> RestoreDriverAsync(int id)
    {
        try
        {
            var request = new RestoreDriverRequest { Id = id };
            var response = await _driversClient.RestoreDriverAsync(request);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error restoring driver with ID: {DriverId}", id);
            throw new Exception($"Failed to restore driver: {ex.Status.Detail}");
        }
    }

    public async Task<GetDeletedDriversResponse> GetDeletedDriversAsync()
    {
        try
        {
            var request = new GetDeletedDriversRequest();
            var response = await _driversClient.GetDeletedDriversAsync(request);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error getting deleted drivers");
            throw new Exception($"Failed to get deleted drivers: {ex.Status.Detail}");
        }
    }

    public async Task<HardDeleteDriverResponse> HardDeleteDriverAsync(int id)
    {
        try
        {
            var request = new HardDeleteDriverRequest { Id = id };
            var response = await _driversClient.HardDeleteDriverAsync(request);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error hard deleting driver with ID: {DriverId}", id);
            throw new Exception($"Failed to hard delete driver: {ex.Status.Detail}");
        }
    }
}

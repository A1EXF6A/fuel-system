using Grpc.Core;
using XYZ.DriversService.Application.Interfaces;
using XYZ.DriversService.Domain.Enums;
using XYZ.DriversService.Protos;
using XYZ.DriversService.Shared.Dtos;

namespace XYZ.DriversService.Controllers;

public class DriversGrpcService : Drivers.DriversBase
{
    private readonly IDriverService _driverService;
    private readonly ILogger<DriversGrpcService> _logger;

    public DriversGrpcService(IDriverService driverService, ILogger<DriversGrpcService> logger)
    {
        _driverService = driverService;
        _logger = logger;
    }

    public override async Task<CreateDriverResponse> CreateDriver(CreateDriverRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Creating driver with document number: {DocumentNumber}", request.DocumentNumber);

            var createDto = new CreateDriverRequestDto
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DocumentNumber = request.DocumentNumber,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                LicenseNumber = request.LicenseNumber,
                LicenseCategory = (LicenseCategory)request.LicenseCategory,
                LicenseExpiryDate = request.LicenseExpiryDate.ToDateTime(),
                DriverType = (DriverType)request.DriverType,
                HireDate = request.HireDate.ToDateTime()
            };

            var driver = await _driverService.CreateDriverAsync(createDto);

            return new CreateDriverResponse
            {
                Driver = MapToProtoDriver(driver),
                Success = true
            };
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Failed to create driver: {Error}", ex.Message);
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating driver");
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while creating the driver"));
        }
    }

    public override async Task<GetDriverResponse> GetDriver(GetDriverRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Getting driver with ID: {DriverId}", request.Id);

            var driver = await _driverService.GetDriverByIdAsync(request.Id);

            if (driver == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Driver with ID {request.Id} not found"));
            }

            return new GetDriverResponse
            {
                Driver = MapToProtoDriver(driver)
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting driver with ID: {DriverId}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while retrieving the driver"));
        }
    }

    public override async Task<GetAllDriversResponse> GetAllDrivers(GetAllDriversRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Getting all drivers");

            var drivers = await _driverService.GetAllDriversAsync();

            var response = new GetAllDriversResponse();
            response.Drivers.AddRange(drivers.Select(MapToProtoDriver));

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all drivers");
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while retrieving drivers"));
        }
    }

    public override async Task<UpdateDriverResponse> UpdateDriver(UpdateDriverRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Updating driver with ID: {DriverId}", request.Id);

            var updateDto = new UpdateDriverRequestDto
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                LicenseNumber = request.LicenseNumber,
                LicenseCategory = (LicenseCategory)request.LicenseCategory,
                LicenseExpiryDate = request.LicenseExpiryDate.ToDateTime(),
                DriverType = (DriverType)request.DriverType,
                Status = (DriverStatus)request.Status
            };

            var driver = await _driverService.UpdateDriverAsync(request.Id, updateDto);

            if (driver == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Driver with ID {request.Id} not found"));
            }

            return new UpdateDriverResponse
            {
                Driver = MapToProtoDriver(driver),
                Success = true
            };
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Failed to update driver: {Error}", ex.Message);
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating driver with ID: {DriverId}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while updating the driver"));
        }
    }

    public override async Task<DeleteDriverResponse> DeleteDriver(DeleteDriverRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Deleting driver with ID: {DriverId}", request.Id);

            var success = await _driverService.DeleteDriverAsync(request.Id);

            if (!success)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Driver with ID {request.Id} not found"));
            }

            return new DeleteDriverResponse
            {
                Success = true
            };
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Failed to delete driver: {Error}", ex.Message);
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting driver with ID: {DriverId}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while deleting the driver"));
        }
    }

    public override async Task<GetAvailableDriversResponse> GetAvailableDrivers(GetAvailableDriversRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Getting available drivers");

            var drivers = await _driverService.GetAvailableDriversAsync();

            var response = new GetAvailableDriversResponse();
            response.Drivers.AddRange(drivers.Select(MapToProtoDriver));

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available drivers");
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while retrieving available drivers"));
        }
    }

    public override async Task<AssignDriverResponse> AssignDriver(AssignDriverRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Assigning driver {DriverId} to vehicle {VehicleId}", request.DriverId, request.VehicleId);

            var success = await _driverService.AssignDriverAsync(request.DriverId, request.VehicleId);

            if (!success)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Driver with ID {request.DriverId} not found"));
            }

            return new AssignDriverResponse
            {
                Success = true
            };
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Failed to assign driver: {Error}", ex.Message);
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning driver {DriverId} to vehicle {VehicleId}", request.DriverId, request.VehicleId);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while assigning the driver"));
        }
    }

    public override async Task<UnassignDriverResponse> UnassignDriver(UnassignDriverRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Unassigning driver with ID: {DriverId}", request.DriverId);

            var success = await _driverService.UnassignDriverAsync(request.DriverId);

            if (!success)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Driver with ID {request.DriverId} not found"));
            }

            return new UnassignDriverResponse
            {
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unassigning driver with ID: {DriverId}", request.DriverId);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while unassigning the driver"));
        }
    }

    private static Driver MapToProtoDriver(DriverResponseDto dto)
    {
        var driver = new Driver
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DocumentNumber = dto.DocumentNumber,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            LicenseNumber = dto.LicenseNumber,
            LicenseCategory = (int)dto.LicenseCategory,
            DriverType = (int)dto.DriverType,
            Status = (int)dto.Status,
            IsAssigned = dto.IsAssigned
        };

        driver.LicenseExpiryDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(dto.LicenseExpiryDate.ToUniversalTime());
        driver.HireDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(dto.HireDate.ToUniversalTime());
        driver.CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(dto.CreatedAt.ToUniversalTime());
        driver.UpdatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(dto.UpdatedAt.ToUniversalTime());

        if (!string.IsNullOrEmpty(dto.AssignedVehicleId))
        {
            driver.AssignedVehicleId = dto.AssignedVehicleId;
        }

        if (dto.AssignmentDate.HasValue)
        {
            driver.AssignmentDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(dto.AssignmentDate.Value.ToUniversalTime());
        }

        return driver;
    }
}
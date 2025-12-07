using Grpc.Net.Client;
using XYZ.DriversService.Application.Interfaces;
using XYZ.DriversService.Domain.Entities;
using XYZ.DriversService.Domain.Enums;
using XYZ.DriversService.Infrastructure.Repositories;
using XYZ.DriversService.Shared.Dtos;
using XYZ.VehiclesService.Protos;

namespace XYZ.DriversService.Application.Services;

public class DriverService : IDriverService
{
    private readonly DriverRepository _driverRepository;

    private readonly Vehicles.VehiclesClient _vehiclesClient;

    public DriverService(IConfiguration configuration, DriverRepository driverRepository)
    {
        _driverRepository = driverRepository;

        var vehiclesUrl =
            configuration.GetValue<string>("Services:VehiclesService")
            ?? "http://vehiclesservice:5002";

        var channel = GrpcChannel.ForAddress(vehiclesUrl);
        _vehiclesClient = new Vehicles.VehiclesClient(channel);
    }

    public async Task<DriverResponseDto?> GetDriverByIdAsync(int id)
    {
        var driver = await _driverRepository.GetByIdAsync(id);
        return driver != null ? MapToDto(driver) : null;
    }

    public async Task<List<DriverResponseDto>> GetAllDriversAsync()
    {
        var drivers = await _driverRepository.GetAllAsync();
        return [.. drivers.Select(MapToDto)];
    }

    public async Task<List<DriverResponseDto>> GetAvailableDriversAsync()
    {
        var drivers = await _driverRepository.GetAvailableAsync();
        return [.. drivers.Select(MapToDto)];
    }

    public async Task<List<DriverResponseDto>> GetDriversByTypeAsync(DriverType driverType)
    {
        var drivers = await _driverRepository.GetByDriverTypeAsync(driverType);
        return [.. drivers.Select(MapToDto)];
    }

    public async Task<DriverResponseDto> CreateDriverAsync(CreateDriverRequestDto request)
    {
        // Validate unique constraints
        if (await _driverRepository.DocumentNumberExistsAsync(request.DocumentNumber))
            throw new InvalidOperationException(
                $"Driver with document number {request.DocumentNumber} already exists"
            );

        if (await _driverRepository.EmailExistsAsync(request.Email))
            throw new InvalidOperationException(
                $"Driver with email {request.Email} already exists"
            );

        if (await _driverRepository.LicenseNumberExistsAsync(request.LicenseNumber))
            throw new InvalidOperationException(
                $"Driver with license number {request.LicenseNumber} already exists"
            );

        // Validate license expiry date
        if (request.LicenseExpiryDate <= DateTime.UtcNow)
            throw new InvalidOperationException("License expiry date must be in the future");

        var driver = new Driver
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            DocumentNumber = request.DocumentNumber,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            LicenseNumber = request.LicenseNumber,
            LicenseCategory = request.LicenseCategory,
            LicenseExpiryDate = request.LicenseExpiryDate,
            DriverType = request.DriverType,
            Status = DriverStatus.Active,
            HireDate = request.HireDate,
        };

        var createdDriver = await _driverRepository.CreateAsync(driver);
        return MapToDto(createdDriver);
    }

    public async Task<DriverResponseDto?> UpdateDriverAsync(int id, UpdateDriverRequestDto request)
    {
        var existingDriver = await _driverRepository.GetByIdAsync(id);
        if (existingDriver == null)
            return null;

        // Validate unique constraints (excluding current driver)
        if (await _driverRepository.EmailExistsAsync(request.Email, id))
            throw new InvalidOperationException(
                $"Driver with email {request.Email} already exists"
            );

        if (await _driverRepository.LicenseNumberExistsAsync(request.LicenseNumber, id))
            throw new InvalidOperationException(
                $"Driver with license number {request.LicenseNumber} already exists"
            );

        // Validate license expiry date
        if (request.LicenseExpiryDate <= DateTime.UtcNow)
            throw new InvalidOperationException("License expiry date must be in the future");

        // Update properties
        existingDriver.FirstName = request.FirstName;
        existingDriver.LastName = request.LastName;
        existingDriver.PhoneNumber = request.PhoneNumber;
        existingDriver.Email = request.Email;
        existingDriver.LicenseNumber = request.LicenseNumber;
        existingDriver.LicenseCategory = request.LicenseCategory;
        existingDriver.LicenseExpiryDate = request.LicenseExpiryDate;
        existingDriver.DriverType = request.DriverType;
        existingDriver.Status = request.Status;

        var updatedDriver = await _driverRepository.UpdateAsync(existingDriver);
        return MapToDto(updatedDriver);
    }

    public async Task<bool> DeleteDriverAsync(int id)
    {
        var driver = await _driverRepository.GetByIdAsync(id);
        if (driver == null)
            return false;

        // If driver is assigned to a vehicle, try to clear the vehicle's assigned driver first
        if (driver.IsAssigned && !string.IsNullOrWhiteSpace(driver.AssignedVehiclePlaca))
        {
            try
            {
                await _vehiclesClient.SetAssignedDriverAsync(
                    new SetAssignedDriverRequest
                    {
                        Placa = driver.AssignedVehiclePlaca,
                        DriverDocument = "",
                    }
                );
            }
            catch
            {
                // ignore failures to clear vehicle; proceed with deletion
            }
        }

        return await _driverRepository.DeleteAsync(id);
    }

    public async Task<DriverResponseDto?> GetDriverByDocumentNumberAsync(string documentNumber)
    {
        var driver = await _driverRepository.GetByDocumentNumberAsync(documentNumber);
        return driver != null ? MapToDto(driver) : null;
    }

    public async Task<bool> AssignDriverAsync(int driverId, string vehiclePlaca)
    {
        var driver = await _driverRepository.GetByIdAsync(driverId);
        if (driver == null)
            return false;

        if (driver.Status != DriverStatus.Active)
            throw new InvalidOperationException("Only active drivers can be assigned");

        if (driver.IsAssigned)
            throw new InvalidOperationException("Driver is already assigned to a vehicle");

        VehicleResponse vehicleResp;
        try
        {
            vehicleResp = await _vehiclesClient.GetVehicleByPlacaAsync(
                new GetVehicleByPlacaRequest { Placa = vehiclePlaca }
            );
        }
        catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            throw new InvalidOperationException("Vehicle not found");
        }

        // If vehicle is assigned to another driver, reject
        if (
            !string.IsNullOrWhiteSpace(vehicleResp.AssignedDriverDocument)
            && vehicleResp.AssignedDriverDocument != driver.DocumentNumber
        )
            throw new InvalidOperationException("Vehicle is already assigned to another driver");

        // Proceed: set vehicle assigned driver document and update driver
        try
        {
            await _vehiclesClient.SetAssignedDriverAsync(
                new SetAssignedDriverRequest
                {
                    Placa = vehiclePlaca,
                    DriverDocument = driver.DocumentNumber,
                }
            );
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to assign vehicle: " + ex.Message);
        }

        driver.IsAssigned = true;
        driver.AssignedVehiclePlaca = vehiclePlaca;
        driver.AssignmentDate = DateTime.UtcNow;

        await _driverRepository.UpdateAsync(driver);
        return true;
    }

    public async Task<bool> UnassignDriverAsync(int driverId)
    {
        var driver = await _driverRepository.GetByIdAsync(driverId);
        if (driver == null)
            return false;

        if (!driver.IsAssigned)
            return true; // Already unassigned

        // Clear vehicle assigned document if vehicle exists
        if (!string.IsNullOrWhiteSpace(driver.AssignedVehiclePlaca))
        {
            try
            {
                await _vehiclesClient.SetAssignedDriverAsync(
                    new SetAssignedDriverRequest
                    {
                        Placa = driver.AssignedVehiclePlaca,
                        DriverDocument = "",
                    }
                );
            }
            catch
            {
                // ignore failures to clear vehicle; still proceed to unassign driver record
            }
        }

        driver.IsAssigned = false;
        driver.AssignedVehiclePlaca = null;
        driver.AssignmentDate = null;

        await _driverRepository.UpdateAsync(driver);
        return true;
    }

    public async Task<bool> DriverExistsAsync(int id)
    {
        return await _driverRepository.ExistsAsync(id);
    }

    private static DriverResponseDto MapToDto(Driver driver)
    {
        return new DriverResponseDto
        {
            Id = driver.Id,
            FirstName = driver.FirstName,
            LastName = driver.LastName,
            DocumentNumber = driver.DocumentNumber,
            PhoneNumber = driver.PhoneNumber,
            Email = driver.Email,
            LicenseNumber = driver.LicenseNumber,
            LicenseCategory = driver.LicenseCategory,
            LicenseExpiryDate = driver.LicenseExpiryDate,
            DriverType = driver.DriverType,
            Status = driver.Status ?? DriverStatus.Active,
            HireDate = driver.HireDate,
            CreatedAt = driver.CreatedAt,
            UpdatedAt = driver.UpdatedAt,
            IsAssigned = driver.IsAssigned,
            AssignedVehiclePlaca = driver.AssignedVehiclePlaca,
            AssignmentDate = driver.AssignmentDate,
        };
    }
}


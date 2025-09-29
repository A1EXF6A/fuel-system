using XYZ.DriversService.Application.Interfaces;
using XYZ.DriversService.Domain.Entities;
using XYZ.DriversService.Domain.Enums;
using XYZ.DriversService.Infrastructure.Repositories;
using XYZ.DriversService.Shared.Dtos;

namespace XYZ.DriversService.Application.Services;

public class DriverService : IDriverService
{
    private readonly DriverRepository _driverRepository;

    public DriverService(DriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task<DriverResponseDto?> GetDriverByIdAsync(int id)
    {
        var driver = await _driverRepository.GetByIdAsync(id);
        return driver != null ? MapToDto(driver) : null;
    }

    public async Task<List<DriverResponseDto>> GetAllDriversAsync()
    {
        var drivers = await _driverRepository.GetAllAsync();
        return drivers.Select(MapToDto).ToList();
    }

    public async Task<List<DriverResponseDto>> GetAvailableDriversAsync()
    {
        var drivers = await _driverRepository.GetAvailableAsync();
        return drivers.Select(MapToDto).ToList();
    }

    public async Task<List<DriverResponseDto>> GetDriversByTypeAsync(DriverType driverType)
    {
        var drivers = await _driverRepository.GetByDriverTypeAsync(driverType);
        return drivers.Select(MapToDto).ToList();
    }

    public async Task<DriverResponseDto> CreateDriverAsync(CreateDriverRequestDto request)
    {
        // Validate unique constraints
        if (await _driverRepository.DocumentNumberExistsAsync(request.DocumentNumber))
            throw new InvalidOperationException($"Driver with document number {request.DocumentNumber} already exists");

        if (await _driverRepository.EmailExistsAsync(request.Email))
            throw new InvalidOperationException($"Driver with email {request.Email} already exists");

        if (await _driverRepository.LicenseNumberExistsAsync(request.LicenseNumber))
            throw new InvalidOperationException($"Driver with license number {request.LicenseNumber} already exists");

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
            HireDate = request.HireDate
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
            throw new InvalidOperationException($"Driver with email {request.Email} already exists");

        if (await _driverRepository.LicenseNumberExistsAsync(request.LicenseNumber, id))
            throw new InvalidOperationException($"Driver with license number {request.LicenseNumber} already exists");

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

        // Check if driver is currently assigned
        if (driver.IsAssigned)
            throw new InvalidOperationException("Cannot delete an assigned driver. Please unassign first.");

        return await _driverRepository.DeleteAsync(id);
    }

    public async Task<bool> AssignDriverAsync(int driverId, string vehicleId)
    {
        var driver = await _driverRepository.GetByIdAsync(driverId);
        if (driver == null)
            return false;

        if (driver.Status != DriverStatus.Active)
            throw new InvalidOperationException("Only active drivers can be assigned");

        if (driver.IsAssigned)
            throw new InvalidOperationException("Driver is already assigned to a vehicle");

        driver.IsAssigned = true;
        driver.AssignedVehicleId = vehicleId;
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

        driver.IsAssigned = false;
        driver.AssignedVehicleId = null;
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
            Status = driver.Status,
            HireDate = driver.HireDate,
            CreatedAt = driver.CreatedAt,
            UpdatedAt = driver.UpdatedAt,
            IsAssigned = driver.IsAssigned,
            AssignedVehicleId = driver.AssignedVehicleId,
            AssignmentDate = driver.AssignmentDate
        };
    }
}
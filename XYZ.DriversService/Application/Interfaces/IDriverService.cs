using XYZ.DriversService.Domain.Entities;
using XYZ.DriversService.Domain.Enums;
using XYZ.DriversService.Shared.Dtos;

namespace XYZ.DriversService.Application.Interfaces;

public interface IDriverService
{
    Task<DriverResponseDto?> GetDriverByIdAsync(int id);
    Task<List<DriverResponseDto>> GetAllDriversAsync();
    Task<List<DriverResponseDto>> GetAvailableDriversAsync();
    Task<List<DriverResponseDto>> GetDriversByTypeAsync(DriverType driverType);
    Task<DriverResponseDto> CreateDriverAsync(CreateDriverRequestDto request);
    Task<DriverResponseDto?> UpdateDriverAsync(int id, UpdateDriverRequestDto request);
    Task<bool> DeleteDriverAsync(int id, string deletedBy, string? reason = null);
    Task<bool> RestoreDriverAsync(int id);
    Task<List<DriverResponseDto>> GetDeletedDriversAsync();
    Task<DriverResponseDto?> GetDeletedDriverByIdAsync(int id);
    Task<bool> HardDeleteDriverAsync(int id);
    Task<bool> AssignDriverAsync(int driverId, string vehicleId);
    Task<bool> UnassignDriverAsync(int driverId);
    Task<bool> DriverExistsAsync(int id);
}
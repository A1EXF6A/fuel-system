using XYZ.VehiclesService.Domain.Entities;

namespace XYZ.VehiclesService.Application.Interfaces;

public interface IVehicleService
{
    Task<Vehicle> CreateAsync(Vehicle vehicle);
    Task<Vehicle?> GetByPlacaAsync(string placa);
    Task<Vehicle?> GetByIdAsync(int id);
    Task<List<Vehicle>> GetAllAsync();
    Task<List<VehicleType>> GetVehicleTypesAsync();
}

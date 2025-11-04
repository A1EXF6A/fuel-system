using XYZ.VehiclesService.Application.Interfaces;
using XYZ.VehiclesService.Domain.Entities;
using XYZ.VehiclesService.Infrastructure.Repositories;

namespace XYZ.VehiclesService.Application.Services;

public class VehicleService : IVehicleService
{
    private readonly VehicleRepository _vehicleRepo;
    private readonly VehicleTypeRepository _typeRepo;

    public VehicleService(VehicleRepository vehicleRepo, VehicleTypeRepository typeRepo)
    {
        _vehicleRepo = vehicleRepo;
        _typeRepo = typeRepo;
    }

    public Task<Vehicle> CreateAsync(Vehicle v) => _vehicleRepo.AddAsync(v);
    public Task<Vehicle?> GetByPlacaAsync(string placa) => _vehicleRepo.GetByPlacaAsync(placa);
    public Task<Vehicle?> GetByIdAsync(int id) => _vehicleRepo.GetByIdAsync(id);
    public Task<List<Vehicle>> GetAllAsync() => _vehicleRepo.GetAllAsync();
    public Task<List<VehicleType>> GetVehicleTypesAsync() => _typeRepo.GetAllAsync();
}

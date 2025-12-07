using Grpc.Net.Client;
using XYZ.DriversService.Protos;
using XYZ.VehiclesService.Application.Interfaces;
using XYZ.VehiclesService.Domain.Entities;
using XYZ.VehiclesService.Infrastructure.Repositories;
using XYZ.VehiclesService.Protos;

namespace XYZ.VehiclesService.Application.Services;

public class VehicleService : IVehicleService
{
    private readonly VehicleRepository _vehicleRepo;
    private readonly VehicleTypeRepository _typeRepo;

    private readonly Drivers.DriversClient _driversClient;

    public VehicleService(
        IConfiguration configuration,
        VehicleRepository vehicleRepo,
        VehicleTypeRepository typeRepo
    )
    {
        string driversUrl =
            configuration.GetValue<string>("Services:DriversService")
            ?? "http://driversservice:5002";

        var channel = GrpcChannel.ForAddress(driversUrl);
        _driversClient = new Drivers.DriversClient(channel);

        _vehicleRepo = vehicleRepo;
        _typeRepo = typeRepo;
    }

    public Task<Vehicle> CreateAsync(Vehicle v) => _vehicleRepo.AddAsync(v);

    public Task<Vehicle?> GetByPlacaAsync(string placa) => _vehicleRepo.GetByPlacaAsync(placa);

    public Task<Vehicle?> GetByIdAsync(int id) => _vehicleRepo.GetByIdAsync(id);

    public Task<List<Vehicle>> GetAllAsync() => _vehicleRepo.GetAllAsync();

    public Task<List<VehicleType>> GetVehicleTypesAsync() => _typeRepo.GetAllAsync();

    public Task<Vehicle?> UpdateAssignedDriverDocumentAsync(string placa, string? driverDocument) =>
        _vehicleRepo.UpdateAssignedDriverDocumentAsync(placa, driverDocument);

    public Task<Vehicle?> UpdateAsync(Vehicle vehicle) => _vehicleRepo.UpdateAsync(vehicle);

    public async Task<bool> DeleteAsync(int id)
    {
        var v = await _vehicleRepo.GetByIdAsync(id);
        if (v == null)
            return false;

        // If vehicle is assigned to a driver, try to unassign the driver via DriversService
        if (!string.IsNullOrWhiteSpace(v.AssignedDriverDocument))
        {
            try
            {
                var getResp = await _driversClient.GetDriverByDocumentNumberAsync(
                    new GetDriverByDocumentNumberRequest
                    {
                        DocumentNumber = v.AssignedDriverDocument,
                    }
                );
                if (getResp != null && getResp.Driver != null)
                {
                    // Unassign driver
                    await _driversClient.UnassignDriverAsync(
                        new UnassignDriverRequest { DriverId = getResp.Driver.Id }
                    );
                }
            }
            catch
            {
                // ignore errors and continue with deletion
            }
        }

        return await _vehicleRepo.DeleteAsync(id);
    }
}

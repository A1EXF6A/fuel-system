using Grpc.Core;
using XYZ.VehiclesService.Application.Interfaces;
using XYZ.VehiclesService.Domain.Entities;
using XYZ.VehiclesService.Protos;

namespace XYZ.VehiclesService.Controllers;

public class VehiclesGrpcService : Vehicles.VehiclesBase
{
    private readonly IVehicleService _service;

    public VehiclesGrpcService(IVehicleService service)
    {
        _service = service;
    }

    public override async Task<VehicleResponse> CreateVehicle(CreateVehicleRequest request, ServerCallContext context)
    {
        // Verificar unicidad de placa
        var existing = await _service.GetByPlacaAsync(request.Placa);
        if (existing != null)
            throw new RpcException(new Status(StatusCode.AlreadyExists, "Vehicle with the same placa already exists"));

        var vehicle = new Vehicle
        {
            Placa = request.Placa,
            Chasis = request.Chasis,
            Marca = request.Marca,
            Modelo = request.Modelo,
            Anio = request.Anio,
            VehicleTypeId = request.VehicleTypeId,
            Estado = request.Estado,
            Km = request.Km
            ,AssignedDriverDocument = string.IsNullOrWhiteSpace(request.AssignedDriverDocument) ? null : request.AssignedDriverDocument
        };

        var created = await _service.CreateAsync(vehicle);

        return new VehicleResponse
        {
            Id = created.Id,
            Placa = created.Placa,
            Marca = created.Marca,
            Modelo = created.Modelo,
            TipoMaquinaria = created.VehicleType?.TipoMaquinaria.ToString() ?? "",
            TipoMotor = created.VehicleType?.TipoMotor ?? "",
            ConsumoBase = created.VehicleType?.ConsumoBase ?? 0,
            Estado = created.Estado,
            Km = created.Km,
            AssignedDriverDocument = created.AssignedDriverDocument ?? ""
        };
    }

    public override async Task<VehicleResponse> GetVehicleByPlaca(GetVehicleByPlacaRequest request, ServerCallContext context)
    {
        var v = await _service.GetByPlacaAsync(request.Placa);
        if (v == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Vehicle not found"));

        return new VehicleResponse
        {
            Id = v.Id,
            Placa = v.Placa,
            Marca = v.Marca,
            Modelo = v.Modelo,
            TipoMaquinaria = v.VehicleType?.TipoMaquinaria.ToString() ?? "",
            TipoMotor = v.VehicleType?.TipoMotor ?? "",
            ConsumoBase = v.VehicleType?.ConsumoBase ?? 0,
            Estado = v.Estado,
            Km = v.Km,
            AssignedDriverDocument = v.AssignedDriverDocument ?? ""
        };
    }

    public override async Task<VehicleResponse> SetAssignedDriver(SetAssignedDriverRequest request, ServerCallContext context)
    {
        var v = await _service.UpdateAssignedDriverDocumentAsync(request.Placa, string.IsNullOrWhiteSpace(request.DriverDocument) ? null : request.DriverDocument);
        if (v == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Vehicle not found"));

        return new VehicleResponse
        {
            Id = v.Id,
            Placa = v.Placa,
            Marca = v.Marca,
            Modelo = v.Modelo,
            TipoMaquinaria = v.VehicleType?.TipoMaquinaria.ToString() ?? "",
            TipoMotor = v.VehicleType?.TipoMotor ?? "",
            ConsumoBase = v.VehicleType?.ConsumoBase ?? 0,
            Estado = v.Estado,
            Km = v.Km,
            AssignedDriverDocument = v.AssignedDriverDocument ?? ""
        };
    }

    public override async Task<VehicleResponse> GetVehicleById(GetVehicleRequest request, ServerCallContext context)
    {
        var v = await _service.GetByIdAsync(request.Id);
        if (v == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Vehicle not found"));

        return new VehicleResponse
        {
            Id = v.Id,
            Placa = v.Placa,
            Marca = v.Marca,
            Modelo = v.Modelo,
            TipoMaquinaria = v.VehicleType?.TipoMaquinaria.ToString() ?? "",
            TipoMotor = v.VehicleType?.TipoMotor ?? "",
            ConsumoBase = v.VehicleType?.ConsumoBase ?? 0,
            Estado = v.Estado,
            Km = v.Km,
            AssignedDriverDocument = v.AssignedDriverDocument ?? ""
        };
    }

    public override async Task<VehiclesListResponse> GetAllVehicles(EmptyRequest request, ServerCallContext context)
    {
        var list = await _service.GetAllAsync();
        var response = new VehiclesListResponse();
        response.Vehicles.AddRange(list.Select(v => new VehicleResponse
        {
            Id = v.Id,
            Placa = v.Placa,
            Marca = v.Marca,
            Modelo = v.Modelo,
            TipoMaquinaria = v.VehicleType?.TipoMaquinaria.ToString() ?? "",
            TipoMotor = v.VehicleType?.TipoMotor ?? "",
            ConsumoBase = v.VehicleType?.ConsumoBase ?? 0,
            Estado = v.Estado,
            Km = v.Km,
            AssignedDriverDocument = v.AssignedDriverDocument ?? ""
        }));
        return response;
    }

    public override async Task<VehicleTypesListResponse> GetVehicleTypes(EmptyRequest request, ServerCallContext context)
    {
        var types = await _service.GetVehicleTypesAsync();
        var resp = new VehicleTypesListResponse();
        resp.Types_.AddRange(types.Select(t => new VehicleTypeResponse
        {
            Id = t.Id,
            TipoMaquinaria = t.TipoMaquinaria.ToString(),
            Subtipo = t.Subtipo,
            TipoMotor = t.TipoMotor,
            ConsumoBase = t.ConsumoBase,
            FactorMotor = t.FactorMotor,
            Pavimentado = t.Pavimentado,
            Montanoso = t.Montanoso,
            Mixto = t.Mixto
        }));
        return resp;
    }
}

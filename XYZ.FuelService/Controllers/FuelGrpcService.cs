using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using XYZ.FuelService.Application.Interfaces;
using XYZ.FuelService.Protos;

namespace XYZ.FuelService.Controllers;

public class FuelGrpcService : Fuel.FuelBase
{
    private readonly IFuelService _service;

    public FuelGrpcService(IFuelService service)
    {
        _service = service;
    }

    public override async Task<FuelPlanResponse> CreateFuelPlan(FuelPlanRequest request, ServerCallContext context)
    {
        var plan = await _service.CreateFuelPlanAsync(request.VehiclePlaca, request.DriverId, request.RouteId);

        return new FuelPlanResponse
        {
            Id = plan.Id,
            VehiclePlaca = plan.VehiclePlaca,
            DriverId = plan.DriverId,
            RouteId = plan.RouteId,
            EstimatedLiters = plan.EstimatedLiters,
            ActualLiters = plan.ActualLiters,
            Estado = plan.Estado.ToString(),
            TipoMaquinaria = plan.TipoMaquinaria
        };
    }

    public override async Task<FuelPlanResponse> RegisterActualConsumption(ActualConsumptionRequest request, ServerCallContext context)
    {
        var plan = await _service.RegisterActualConsumptionAsync(request.PlanId, request.ActualLiters);
        if (plan == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Plan not found"));

        return new FuelPlanResponse
        {
            Id = plan.Id,
            VehiclePlaca = plan.VehiclePlaca,
            DriverId = plan.DriverId,
            RouteId = plan.RouteId,
            EstimatedLiters = plan.EstimatedLiters,
            ActualLiters = plan.ActualLiters,
            Estado = plan.Estado.ToString()
        };
    }

    public override async Task<FuelReportResponse> GetFuelReport(FuelReportRequest request, ServerCallContext context)
    {
        var list = await _service.GetFuelReportAsync(request.FilterType, request.FilterValue);
        var resp = new FuelReportResponse();
        resp.Registros.AddRange(list.Select(f => new FuelPlanResponse
        {
            Id = f.Id,
            VehiclePlaca = f.VehiclePlaca,
            DriverId = f.DriverId,
            RouteId = f.RouteId,
            EstimatedLiters = f.EstimatedLiters,
            ActualLiters = f.ActualLiters,
            Estado = f.Estado.ToString(),
            TipoMaquinaria = f.TipoMaquinaria
        }));
        return resp;
    }

    public override async Task<FuelReportResponse> GetAllFuelReports(EmptyRequest request, ServerCallContext context)
    {
        var list = await _service.GetAllReportsAsync();
        var resp = new FuelReportResponse();
        resp.Registros.AddRange(list.Select(f => new FuelPlanResponse
        {
            Id = f.Id,
            VehiclePlaca = f.VehiclePlaca,
            DriverId = f.DriverId,
            RouteId = f.RouteId,
            EstimatedLiters = f.EstimatedLiters,
            ActualLiters = f.ActualLiters,
            Estado = f.Estado.ToString(),
            TipoMaquinaria = f.TipoMaquinaria
        }));
        return resp;
    }

    public override async Task<FuelPlanResponse> UpdateReportStatus(UpdateReportStatusRequest request, ServerCallContext context)
    {
        var updated = await _service.UpdateReportStatusAsync(request.Id, request.Estado);
        if (updated == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Fuel report not found"));

        return new FuelPlanResponse
        {
            Id = updated.Id,
            VehiclePlaca = updated.VehiclePlaca,
            DriverId = updated.DriverId,
            RouteId = updated.RouteId,
            EstimatedLiters = updated.EstimatedLiters,
            ActualLiters = updated.ActualLiters,
            Estado = updated.Estado.ToString(),
            TipoMaquinaria = updated.TipoMaquinaria
        };
    }
}

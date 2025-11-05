using System.Threading.Tasks;
using System.Collections.Generic;
using XYZ.FuelService.Domain.Entities;

namespace XYZ.FuelService.Application.Interfaces;

public interface IFuelService
{
    // vehiclePlaca is optional; if empty, the service will use routeId to determine the vehicle (Routes is source of truth)
    Task<FuelRecord> CreateFuelPlanAsync(string? vehiclePlaca, int driverId, int routeId);
    Task<FuelRecord?> RegisterActualConsumptionAsync(int planId, double actualLiters);
    Task<List<FuelRecord>> GetFuelReportAsync(string filterType, string filterValue);
    Task<List<FuelRecord>> GetAllReportsAsync();
    Task<FuelRecord?> UpdateReportStatusAsync(int id, string estado);
}

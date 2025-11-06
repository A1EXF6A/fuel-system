using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XYZ.ApiGateway.Services;

namespace XYZ.ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FuelController : ControllerBase
{
    private readonly FuelGatewayService _fuelService;
    private readonly DriversGatewayService _driversService;

    public FuelController(FuelGatewayService fuelService, DriversGatewayService driversService)
    {
        _fuelService = fuelService;
        _driversService = driversService;
    }

    [HttpPost("plan")]
    public async Task<IActionResult> CreateFuelPlan([FromBody] CreateFuelPlanDto dto)
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var response = await _fuelService.CreateFuelPlanAsync(dto.VehiclePlaca, dto.DriverId, dto.RouteId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterActualConsumption([FromBody] RegisterConsumptionDto dto)
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                var response = await _fuelService.RegisterActualConsumptionAsync(dto.PlanId, dto.ActualLiters);
                return Ok(response);
            }

            // Operators can register actual consumption only for plans of their assigned vehicle
            if (string.Equals(role, "Operador", StringComparison.OrdinalIgnoreCase))
            {
                var username = User.Identity?.Name ?? string.Empty;
                var driversResp = await _driversService.GetAllDriversAsync();
                var driver = driversResp.Drivers.FirstOrDefault(d => d.DocumentNumber == username);
                if (driver == null || string.IsNullOrEmpty(driver.AssignedVehiclePlaca))
                {
                    return Forbid();
                }

                // Find the plan by id from all reports (no direct GetPlanById RPC available)
                var reports = await _fuelService.GetAllFuelReportsAsync();
                var plan = reports.Registros.FirstOrDefault(r => r.Id == dto.PlanId);
                if (plan == null)
                {
                    return NotFound(new { message = "Fuel plan not found" });
                }

                if (!string.Equals(plan.VehiclePlaca, driver.AssignedVehiclePlaca, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }

                var response = await _fuelService.RegisterActualConsumptionAsync(dto.PlanId, dto.ActualLiters);
                return Ok(response);
            }

            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("report")]
    public async Task<IActionResult> GetFuelReport([FromBody] FuelReportDto dto)
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "Supervisor", StringComparison.OrdinalIgnoreCase))
            {
                var response = await _fuelService.GetFuelReportAsync(dto.FilterType, dto.FilterValue);
                return Ok(response);
            }

            if (string.Equals(role, "Operador", StringComparison.OrdinalIgnoreCase))
            {
                // Operators can only request reports for their assigned vehicle
                if (!string.Equals(dto.FilterType, "vehicle", StringComparison.OrdinalIgnoreCase))
                    return Forbid();

                var username = User.Identity?.Name ?? string.Empty;
                var driversResp = await _driversService.GetAllDriversAsync();
                var driver = driversResp.Drivers.FirstOrDefault(d => d.DocumentNumber == username);
                if (driver == null || string.IsNullOrEmpty(driver.AssignedVehiclePlaca))
                    return Forbid();

                if (!string.Equals(driver.AssignedVehiclePlaca, dto.FilterValue, StringComparison.OrdinalIgnoreCase))
                    return Forbid();

                var response = await _fuelService.GetFuelReportAsync(dto.FilterType, dto.FilterValue);
                return Ok(response);
            }

            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("reports")]
    public async Task<IActionResult> GetAllReports()
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "Supervisor", StringComparison.OrdinalIgnoreCase))
            {
                var response = await _fuelService.GetAllFuelReportsAsync();
                return Ok(response);
            }

            if (string.Equals(role, "Operador", StringComparison.OrdinalIgnoreCase))
            {
                var username = User.Identity?.Name ?? string.Empty;
                var driversResp = await _driversService.GetAllDriversAsync();
                var driver = driversResp.Drivers.FirstOrDefault(d => d.DocumentNumber == username);
                if (driver == null || string.IsNullOrEmpty(driver.AssignedVehiclePlaca))
                {
                    return Ok(new { Registros = new object[0] });
                }

                // Use GetFuelReport with filterType = "vehicle" to retrieve only reports for the assigned vehicle
                var response = await _fuelService.GetFuelReportAsync("vehicle", driver.AssignedVehiclePlaca);
                return Ok(response);
            }

            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("reports/{id}/status")]
    public async Task<IActionResult> UpdateReportStatus(int id, [FromBody] UpdateReportStatusDto request)
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            // Admins and Supervisors can update any report status
            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "Supervisor", StringComparison.OrdinalIgnoreCase))
            {
                var response = await _fuelService.UpdateReportStatusAsync(id, request.Estado);
                return Ok(response);
            }

            // Operators can update report status only for reports of their assigned vehicle
            if (string.Equals(role, "Operador", StringComparison.OrdinalIgnoreCase))
            {
                var username = User.Identity?.Name ?? string.Empty;
                var driversResp = await _driversService.GetAllDriversAsync();
                var driver = driversResp.Drivers.FirstOrDefault(d => d.DocumentNumber == username);
                if (driver == null || string.IsNullOrEmpty(driver.AssignedVehiclePlaca))
                {
                    return Forbid();
                }

                // Retrieve all reports and find the one with the requested id
                var reports = await _fuelService.GetAllFuelReportsAsync();
                var report = reports.Registros.FirstOrDefault(r => r.Id == id);
                if (report == null)
                {
                    return NotFound(new { message = "Fuel report not found" });
                }

                if (!string.Equals(report.VehiclePlaca, driver.AssignedVehiclePlaca, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }

                var response = await _fuelService.UpdateReportStatusAsync(id, request.Estado);
                return Ok(response);
            }

            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record CreateFuelPlanDto(string VehiclePlaca, int DriverId, int RouteId);
public record RegisterConsumptionDto(int PlanId, double ActualLiters);
public record FuelReportDto(string FilterType, string FilterValue);
public record UpdateReportStatusDto(string Estado);

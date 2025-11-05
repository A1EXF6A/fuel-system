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
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var response = await _fuelService.RegisterActualConsumptionAsync(dto.PlanId, dto.ActualLiters);
            return Ok(response);
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
            var response = await _fuelService.GetAllFuelReportsAsync();
            return Ok(response);
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
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) && !string.Equals(role, "Supervisor", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var response = await _fuelService.UpdateReportStatusAsync(id, request.Estado);
            return Ok(response);
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

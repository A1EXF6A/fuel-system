using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XYZ.ApiGateway.Services;
using XYZ.VehiclesService.Protos;

namespace XYZ.ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly VehiclesGatewayService _vehiclesService;
    private readonly DriversGatewayService _driversService;

    public VehiclesController(VehiclesGatewayService vehiclesService, DriversGatewayService driversService)
    {
        _vehiclesService = vehiclesService;
        _driversService = driversService;
    }

    [HttpGet("{placa}")]
    public async Task<IActionResult> GetVehicleByPlaca(string placa)
    {
        try
        {
            var response = await _vehiclesService.GetVehicleByPlacaAsync(placa);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVehicles()
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            // Admin and Supervisor see all
            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "Supervisor", StringComparison.OrdinalIgnoreCase))
            {
                var response = await _vehiclesService.GetAllVehiclesAsync();
                return Ok(response);
            }

            // Operador: only see assigned vehicle
            if (string.Equals(role, "Operador", StringComparison.OrdinalIgnoreCase))
            {
                var username = User.Identity?.Name ?? string.Empty;
                var driversResp = await _driversService.GetAllDriversAsync();
                var driver = driversResp.Drivers.FirstOrDefault(d => d.DocumentNumber == username);
                if (driver == null || string.IsNullOrEmpty(driver.AssignedVehiclePlaca))
                {
                    return Ok(new { Vehicles = new object[0] });
                }

                var vehicle = await _vehiclesService.GetVehicleByPlacaAsync(driver.AssignedVehiclePlaca);
                return Ok(new { Vehicles = new[] { vehicle } });
            }

            // default: deny
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleRequest request)
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var response = await _vehiclesService.CreateVehicleAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{placa}/assign")]
    public async Task<IActionResult> SetAssignedDriver(string placa, [FromBody] SetAssignedDriverDto dto)
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var response = await _vehiclesService.SetAssignedDriverAsync(placa, dto.DriverDocument ?? string.Empty);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record SetAssignedDriverDto(string? DriverDocument);

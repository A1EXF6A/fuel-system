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
    public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleDto dto)
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            // Map DTO to gRPC request
            var request = new CreateVehicleRequest
            {
                Placa = dto.Placa,
                Chasis = dto.Chasis ?? string.Empty,
                Marca = dto.Brand ?? string.Empty,
                Modelo = dto.Model ?? string.Empty,
                Anio = dto.Year,
                VehicleTypeId = dto.VehicleTypeId ?? 1, // Default to 1 if not provided
                Estado = dto.Estado ?? "Activo",
                Km = dto.Km ?? 0,
                AssignedDriverDocument = dto.AssignedDriverDocument ?? string.Empty
            };

            var response = await _vehiclesService.CreateVehicleAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVehicle(int id, [FromBody] UpdateVehicleDto dto)
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            // Map DTO to gRPC request
            var request = new UpdateVehicleRequest
            {
                Id = id,
                Placa = dto.Placa,
                Chasis = dto.Chasis ?? string.Empty,
                Marca = dto.Brand ?? string.Empty,
                Modelo = dto.Model ?? string.Empty,
                Anio = dto.Year,
                VehicleTypeId = dto.VehicleTypeId ?? 1,
                Estado = dto.Estado ?? "Activo",
                Km = dto.Km ?? 0,
                AssignedDriverDocument = dto.AssignedDriverDocument ?? string.Empty
            };

            var response = await _vehiclesService.UpdateVehicleAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var response = await _vehiclesService.DeleteVehicleAsync(id);
            return Ok(new { success = response.Success });
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

public record CreateVehicleDto(
    string Placa,
    string? Chasis,
    string? Brand,
    string? Model,
    int Year,
    string? FuelType,
    int? Capacity,
    double? FuelTankCapacity,
    int? VehicleTypeId,
    string? Estado,
    double? Km,
    string? AssignedDriverDocument
);

public record UpdateVehicleDto(
    int Id,
    string Placa,
    string? Chasis,
    string? Brand,
    string? Model,
    int Year,
    string? FuelType,
    int? Capacity,
    double? FuelTankCapacity,
    int? VehicleTypeId,
    string? Estado,
    double? Km,
    string? AssignedDriverDocument
);

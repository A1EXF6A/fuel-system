using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XYZ.ApiGateway.Services;
using XYZ.RoutesService.Protos;

namespace XYZ.ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoutesController : ControllerBase
{
    private readonly RoutesGatewayService _routesService;
    private readonly DriversGatewayService _driversService;

    public RoutesController(RoutesGatewayService routesService, DriversGatewayService driversService)
    {
        _routesService = routesService;
        _driversService = driversService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRoute([FromBody] CreateRouteDto request)
    {
        try
        {
            var grpcReq = new CreateRouteRequest
            {
                Nombre = request.Nombre,
                Origen = request.Origen,
                Destino = request.Destino,
                VehiclePlaca = request.VehiclePlaca,
                DriverId = request.DriverId
            };

            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var response = await _routesService.CreateRouteAsync(grpcReq);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoutes()
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "Supervisor", StringComparison.OrdinalIgnoreCase))
            {
                var response = await _routesService.GetAllRoutesAsync();
                return Ok(response);
            }

            if (string.Equals(role, "Operador", StringComparison.OrdinalIgnoreCase))
            {
                var username = User.Identity?.Name ?? string.Empty;
                var driversResp = await _driversService.GetAllDriversAsync();
                var driver = driversResp.Drivers.FirstOrDefault(d => d.DocumentNumber == username);
                if (driver == null || string.IsNullOrEmpty(driver.AssignedVehiclePlaca))
                {
                    return Ok(new { Routes = new object[0] });
                }

                var all = await _routesService.GetAllRoutesAsync();
                var filtered = all.Routes.Where(r => r.VehiclePlaca == driver.AssignedVehiclePlaca).ToList();
                return Ok(new { Routes = filtered });
            }

            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRouteById(int id)
    {
        try
        {
            var response = await _routesService.GetRouteByIdAsync(id);
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (string.Equals(role, "Operador", StringComparison.OrdinalIgnoreCase))
            {
                var username = User.Identity?.Name ?? string.Empty;
                var driversResp = await _driversService.GetAllDriversAsync();
                var driver = driversResp.Drivers.FirstOrDefault(d => d.DocumentNumber == username);
                if (driver == null || string.IsNullOrEmpty(driver.AssignedVehiclePlaca))
                {
                    return Forbid();
                }

                if (!string.Equals(response.VehiclePlaca, driver.AssignedVehiclePlaca, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRoute(int id, [FromBody] UpdateRouteDto request)
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var grpcReq = new UpdateRouteRequest
            {
                Id = id,
                Nombre = request.Nombre,
                Origen = request.Origen,
                Destino = request.Destino,
                VehiclePlaca = request.VehiclePlaca,
                DriverId = request.DriverId
            };

            var response = await _routesService.UpdateRouteAsync(grpcReq);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoute(int id)
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var response = await _routesService.DeleteRouteAsync(id);
            return Ok(new { success = response.Success });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/status")]
    public async Task<IActionResult> UpdateRouteStatus(int id, [FromBody] UpdateRouteStatusDto request)
    {
        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) && !string.Equals(role, "Supervisor", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var grpcReq = new UpdateRouteStatusRequest { Id = id, Estado = request.Estado };
            var response = await _routesService.UpdateRouteStatusAsync(grpcReq);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record CreateRouteDto(string Nombre, string Origen, string Destino, string VehiclePlaca, int DriverId);
public record UpdateRouteDto(string Nombre, string Origen, string Destino, string VehiclePlaca, int DriverId);
public record UpdateRouteStatusDto(string Estado);

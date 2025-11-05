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

    public RoutesController(RoutesGatewayService routesService)
    {
        _routesService = routesService;
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
            var response = await _routesService.GetAllRoutesAsync();
            return Ok(response);
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
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record CreateRouteDto(string Nombre, string Origen, string Destino, string VehiclePlaca, int DriverId);

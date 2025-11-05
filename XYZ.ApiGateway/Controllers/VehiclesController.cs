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

    public VehiclesController(VehiclesGatewayService vehiclesService)
    {
        _vehiclesService = vehiclesService;
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
            var response = await _vehiclesService.GetAllVehiclesAsync();
            return Ok(response);
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

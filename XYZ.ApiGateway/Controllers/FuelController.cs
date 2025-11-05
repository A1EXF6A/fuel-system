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

    public FuelController(FuelGatewayService fuelService)
    {
        _fuelService = fuelService;
    }

    [HttpPost("plan")]
    public async Task<IActionResult> CreateFuelPlan([FromBody] CreateFuelPlanDto dto)
    {
        try
        {
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
            var response = await _fuelService.GetFuelReportAsync(dto.FilterType, dto.FilterValue);
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

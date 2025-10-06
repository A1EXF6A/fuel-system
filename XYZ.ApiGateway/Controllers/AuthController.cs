using Microsoft.AspNetCore.Mvc;
using XYZ.ApiGateway.Services;

namespace XYZ.ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthGatewayService _authService;

    public AuthController(AuthGatewayService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request.Username, request.Password);
            
            if (string.IsNullOrEmpty(response.Token))
            {
                return BadRequest(new { message = "Invalid credentials" });
            }
            
            return Ok(new
            {
                success = true,
                token = response.Token,
                refreshToken = response.RefreshToken,
                role = response.Role
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Invalid credentials. Please try again." });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request.Username, request.Password, request.Role);
            return Ok(new
            {
                success = !string.IsNullOrEmpty(response.Token),
                token = response.Token,
                role = response.Role
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenRequestDto request)
    {
        try
        {
            var response = await _authService.ValidateTokenAsync(request.Token);
            return Ok(new
            {
                valid = response.IsValid,
                role = response.Role
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record LoginRequestDto(string Username, string Password);
public record RegisterRequestDto(string Username, string Password, string Role);
public record ValidateTokenRequestDto(string Token);
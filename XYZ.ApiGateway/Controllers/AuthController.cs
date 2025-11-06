using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
        catch (Exception)
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
        catch (Exception)
        {
            return BadRequest(new { message = "Could not register user. Please try again." });
        }
    }

    [HttpPost("validate")]
    [AllowAnonymous]
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
        catch (Exception)
        {
            return BadRequest(new { message = "Could not validate token. Please try again." });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(new
            {
                token = response.Token,
                refreshToken = response.RefreshToken,
                role = response.Role
            });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Could not refresh token. Please try again." });
        }
    }

    [HttpGet("users")]
    public async Task<IActionResult> ListUsers()
    {
        try
        {
            var resp = await _authService.ListUsersAsync();
            var users = resp.Users.Select(u => new { id = u.Id, username = u.Username, role = u.Role });
            return Ok(users);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto request)
    {
        try
        {
            var user = await _authService.UpdateUserAsync(id, request.Username, request.Password, request.Role);
            return Ok(new { id = user.Id, username = user.Username, role = user.Role });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var resp = await _authService.DeleteUserAsync(id);
            return Ok(new { success = resp.Success });
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
public record RefreshRequestDto(string RefreshToken);
public record UpdateUserDto(string Username, string? Password, string Role);
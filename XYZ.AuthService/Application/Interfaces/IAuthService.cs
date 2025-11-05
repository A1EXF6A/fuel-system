using XYZ.AuthService.Shared.Dtos;

namespace XYZ.AuthService.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
    Task<List<UserDto>> ListUsersAsync();
    Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(int id);
    Task<(bool IsValid, string Role)> ValidateTokenAsync(string token);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
}

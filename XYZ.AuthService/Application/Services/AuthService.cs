using System.Text;
using Microsoft.EntityFrameworkCore;
using XYZ.AuthService.Application.Interfaces;
using XYZ.AuthService.Infrastructure.Persistence;
using XYZ.AuthService.Infrastructure.Security;
using XYZ.AuthService.Shared.Dtos;

namespace XYZ.AuthService.Application.Services;

public class AuthService(
    AuthDbContext db,
    PasswordHasher hasher,
    JwtTokenGenerator jwt,
    Infrastructure.Repositories.UserRepository users
) : IAuthService
{
    private readonly AuthDbContext _db = db;
    private readonly PasswordHasher _hasher = hasher;
    private readonly JwtTokenGenerator _jwt = jwt;
    private readonly Infrastructure.Repositories.UserRepository _users = users;

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null || !_hasher.VerifyPassword(dto.Password, user.PasswordHash))
            throw new Exception("Credenciales inválidas");

        var token = _jwt.GenerateToken(user.Username, user.Role.ToString());
        var refresh = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Username));
        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refresh,
            Role = user.Role.ToString(),
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var hash = _hasher.HashPassword(dto.Password);
        var user = new Domain.Entities.User
        {
            Username = dto.Username,
            PasswordHash = hash,
            Role = Enum.Parse<Domain.Enums.UserRole>(dto.Role, true),
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _jwt.GenerateToken(user.Username, user.Role.ToString());
        var refresh = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Username));
        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refresh,
            Role = user.Role.ToString(),
        };
    }

    public async Task<List<UserDto>> ListUsersAsync()
    {
        var users = await _users.ListAsync();
        return users
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role.ToString(),
            })
            .ToList();
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = await _users.GetByIdAsync(id);
        if (user == null)
            return null;

        if (!string.IsNullOrWhiteSpace(dto.Username))
            user.Username = dto.Username;
        if (!string.IsNullOrWhiteSpace(dto.Role))
            user.Role = Enum.Parse<Domain.Enums.UserRole>(dto.Role, true);
        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.PasswordHash = _hasher.HashPassword(dto.Password);

        await _users.UpdateAsync(user);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role.ToString(),
        };
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _users.GetByIdAsync(id);
        if (user == null)
            return false;
        await _users.DeleteAsync(user);
        return true;
    }

    public async Task<(bool IsValid, string Role)> ValidateTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return (false, string.Empty);
        return _jwt.ValidateToken(token);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new Exception("Refresh token inválido");

        try
        {
            var bytes = Convert.FromBase64String(refreshToken);
            var username = Encoding.UTF8.GetString(bytes);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                throw new Exception("Refresh token inválido");

            var token = _jwt.GenerateToken(user.Username, user.Role.ToString());
            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                Role = user.Role.ToString(),
            };
        }
        catch
        {
            throw new Exception("Refresh token inválido");
        }
    }
}

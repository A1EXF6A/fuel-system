using Microsoft.EntityFrameworkCore;
using XYZ.AuthService.Application.Interfaces;
using XYZ.AuthService.Infrastructure.Persistence;
using XYZ.AuthService.Infrastructure.Security;
using XYZ.AuthService.Shared.Dtos;

namespace XYZ.AuthService.Application.Services;

public class AuthService(AuthDbContext db, PasswordHasher hasher, JwtTokenGenerator jwt) : IAuthService
{
    private readonly AuthDbContext _db = db;
    private readonly PasswordHasher _hasher = hasher;
    private readonly JwtTokenGenerator _jwt = jwt;

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null || !_hasher.VerifyPassword(dto.Password, user.PasswordHash))
            throw new Exception("Credenciales inválidas");

        var token = _jwt.GenerateToken(user.Username, user.Role.ToString());
        return new AuthResponseDto { Token = token, RefreshToken = "dummy-refresh", Role = user.Role.ToString() };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var hash = _hasher.HashPassword(dto.Password);
        var user = new Domain.Entities.User
        {
            Username = dto.Username,
            PasswordHash = hash,
            Role = Enum.Parse<Domain.Enums.UserRole>(dto.Role, true)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _jwt.GenerateToken(user.Username, user.Role.ToString());
        return new AuthResponseDto { Token = token, RefreshToken = "dummy-refresh", Role = user.Role.ToString() };
    }
}

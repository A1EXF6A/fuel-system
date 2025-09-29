using System.Threading.Tasks;
using Grpc.Core;
using XYZ.AuthService.Application.Interfaces;
using XYZ.AuthService.Shared.Dtos;
using XYZ.AuthService.Protos;

namespace XYZ.AuthService.Controllers;

public class AuthGrpcService : Auth.AuthBase
{
    private readonly IAuthService _authService;

    public AuthGrpcService(IAuthService authService)
    {
        _authService = authService;
    }

    public override async Task<AuthResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var result = await _authService.LoginAsync(new LoginRequestDto
        {
            Username = request.Username,
            Password = request.Password
        });

        return new AuthResponse
        {
            Token = result.Token,
            RefreshToken = result.RefreshToken,
            Role = result.Role
        };
    }

    public override async Task<AuthResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        var result = await _authService.RegisterAsync(new RegisterRequestDto
        {
            Username = request.Username,
            Password = request.Password,
            Role = request.Role
        });

        return new AuthResponse
        {
            Token = result.Token,
            RefreshToken = result.RefreshToken,
            Role = result.Role
        };
    }
}

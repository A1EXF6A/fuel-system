using System.Threading.Tasks;
using System.Linq;
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

    public override async Task<ListUsersResponse> ListUsers(ListUsersRequest request, ServerCallContext context)
    {
        var users = await _authService.ListUsersAsync();
        var resp = new ListUsersResponse();
        resp.Users.AddRange(users.Select(u => new User { Id = u.Id, Username = u.Username, Role = u.Role }));
        return resp;
    }

    public override async Task<User> UpdateUser(UpdateUserRequest request, ServerCallContext context)
    {
        var update = new UpdateUserDto
        {
            Username = string.IsNullOrWhiteSpace(request.Username) ? null : request.Username,
            Role = string.IsNullOrWhiteSpace(request.Role) ? null : request.Role,
            Password = string.IsNullOrWhiteSpace(request.Password) ? null : request.Password
        };

        var updated = await _authService.UpdateUserAsync(request.Id, update);
        if (updated == null) return new User();
        return new User { Id = updated.Id, Username = updated.Username, Role = updated.Role };
    }

    public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    {
        var ok = await _authService.DeleteUserAsync(request.Id);
        return new DeleteUserResponse { Success = ok };
    }

    public override async Task<ValidateResponse> ValidateToken(ValidateRequest request, ServerCallContext context)
    {
        var res = await _authService.ValidateTokenAsync(request.Token);
        return new ValidateResponse { IsValid = res.IsValid, Role = res.Role };
    }

    public override async Task<AuthResponse> RefreshToken(RefreshRequest request, ServerCallContext context)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);
        return new AuthResponse { Token = result.Token, RefreshToken = result.RefreshToken, Role = result.Role };
    }
}

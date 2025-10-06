using Grpc.Core;
using Grpc.Net.Client;
using XYZ.AuthService.Protos;

namespace XYZ.ApiGateway.Services;

public class AuthGatewayService
{
    private readonly Auth.AuthClient _authClient;
    private readonly ILogger<AuthGatewayService> _logger;

    public AuthGatewayService(IConfiguration configuration, ILogger<AuthGatewayService> logger)
    {
        var authServiceUrl = configuration.GetValue<string>("Services:AuthService");
        var channel = GrpcChannel.ForAddress(authServiceUrl!);
        _authClient = new Auth.AuthClient(channel);
        _logger = logger;
    }

    public async Task<AuthResponse> LoginAsync(string username, string password)
    {
        try
        {
            var request = new LoginRequest
            {
                Username = username,
                Password = password
            };

            var response = await _authClient.LoginAsync(request);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error during login for user: {Username}", username);
            throw new Exception($"Login failed: {ex.Status.Detail}");
        }
    }

    public async Task<AuthResponse> RegisterAsync(string username, string password, string role)
    {
        try
        {
            var request = new RegisterRequest
            {
                Username = username,
                Password = password,
                Role = role
            };

            var response = await _authClient.RegisterAsync(request);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error during registration for user: {Username}", username);
            throw new Exception($"Registration failed: {ex.Status.Detail}");
        }
    }

    public async Task<ValidateResponse> ValidateTokenAsync(string token)
    {
        try
        {
            var request = new ValidateRequest
            {
                Token = token
            };

            var response = await _authClient.ValidateTokenAsync(request);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error during token validation");
            throw new Exception($"Token validation failed: {ex.Status.Detail}");
        }
    }
}

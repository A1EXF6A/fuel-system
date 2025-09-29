using System.Threading.Tasks;
using XYZ.AuthService.Shared.Dtos;

namespace XYZ.AuthService.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
}

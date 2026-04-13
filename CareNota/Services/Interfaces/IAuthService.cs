using CareNota.DTOs.Auth;

namespace CareNota.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto Dto);
    Task<AuthResponseDto> LoginAsync(LoginDto Dto);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto Dto);
    Task RevokeTokenAsync(string UserId);
}
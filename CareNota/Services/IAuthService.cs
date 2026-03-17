using CareNota.DTOs;

namespace CareNota.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto Dto);
    Task<AuthResponseDto> LoginAsync(LoginDto Dto);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto Dto);
    Task RevokeTokenAsync(string UserId);
}
using CarServiceCenter.Application.DTOs.Auth;

namespace CarServiceCenter.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<RefreshTokenResponseDto> RefreshTokenAsync(string refreshToken);
}

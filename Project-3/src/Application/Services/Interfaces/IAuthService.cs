using Microsoft.AspNetCore.Identity;
using Project_3.src.Application.DTOs;

namespace Project_3.src.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<IdentityResult> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> RefreshTokenAsync(TokenRequestDto dto);
        Task<bool> LogoutAsync(string refreshToken);
    }
}

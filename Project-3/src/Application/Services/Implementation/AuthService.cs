using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Project_3.src.Application.Models;
using Project_3.src.Infrastructure.Data.Context;
using Project_3.src.Infrastructure.identity;
using Microsoft.EntityFrameworkCore;
using Project_3.src.Application.Services.Interfaces;
using Project_3.src.Application.DTOs.AuthDto;

namespace Project_3.src.Application.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _context;
        private readonly JwtOptions _jwtOptions;

        public AuthService(
            UserManager<User> userManager,
            ITokenService tokenService,
            AppDbContext context,
            IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _context = context;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Email is already registered."
                });
            }

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return result;

            var roleResult = await _userManager.AddToRoleAsync(user, "Employee");

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);//rollback
                return roleResult;
            }
            var leaveTypes = await _context.Set<LeaveType>().ToListAsync();

            foreach (var leaveType in leaveTypes)
            {
                _context.EmployeeLeaveBalances.Add(new EmployeeLeaveBalance
                {
                    UserId = user.Id,
                    LeaveTypeId = leaveType.Id,
                    RemainingDays = leaveType.DefaultDays
                });
            }

            await _context.SaveChangesAsync();

            return IdentityResult.Success;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return null;

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!isPasswordValid)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            var accessToken = _tokenService.GenerateAccessToken(user, roles);

            var refreshToken = await CreateRefreshTokenAsync(user);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                UserName = user.UserName!,
                Roles = roles.ToList(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresInMinutes)
            };
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(TokenRequestDto dto)
        {
            var storedRefreshToken = await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == dto.RefreshToken);

            if (storedRefreshToken == null || !storedRefreshToken.IsActive)
                return null;

            var user = storedRefreshToken.User;

            if (user == null)
                return null;

            storedRefreshToken.IsRevoked = true;

            var roles = await _userManager.GetRolesAsync(user);

            var accessToken = _tokenService.GenerateAccessToken(user, roles);

            var newRefreshToken = await CreateRefreshTokenAsync(user);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                UserName = user.UserName!,
                Roles = roles.ToList(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresInMinutes)
            };
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshToken);

            if (storedToken == null)
                return false;

            storedToken.IsRevoked = true;

            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<RefreshToken> CreateRefreshTokenAsync(User user)
        {
            var token = new RefreshToken
            {
                Token = _tokenService.GenerateRefreshToken(),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiresInDays),
                IsRevoked = false
            };

            _context.RefreshTokens.Add(token);

            await _context.SaveChangesAsync();

            return token;
        }
    }
}

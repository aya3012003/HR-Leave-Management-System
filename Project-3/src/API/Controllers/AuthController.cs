using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.DTOs.AuthDto;
using Project_3.src.Application.Services.Interfaces;

namespace Project_3.src.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result.Succeeded)
            {

                return BadRequest(new
                {
                    message = "Registration failed.",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            return StatusCode(StatusCodes.Status201Created,
             new { message = "User registered successfully." });
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenRequestDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto);
            if (result == null)
                return BadRequest(new { message = "Invalid or expired refresh token." });

            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDto dto)
        {
            var result = await _authService.LogoutAsync(dto.RefreshToken);
            if (!result)
                return BadRequest(new { message = "Invalid token." });

            return Ok(new { message = "Logged out successfully. Token revoked." });
        }
    }
}


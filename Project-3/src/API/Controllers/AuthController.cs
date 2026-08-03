using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_3.src.Application.DTOs.AuthDto;
using Project_3.src.Application.Services.Interfaces;

namespace Project_3.src.API.Controllers
{
    /// <summary>
    /// Handles user authentication and authorization operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">Authentication service.</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates a user and returns access and refresh tokens.
        /// </summary>
        /// <param name="dto">User login credentials.</param>
        /// <returns>JWT access token and refresh token.</returns>
        /// <response code="200">Login successful.</response>
        /// <response code="401">Invalid email or password.</response>
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

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="dto">Registration information.</param>
        /// <returns>Confirmation message.</returns>
        /// <response code="201">User registered successfully.</response>
        /// <response code="400">Registration failed.</response>
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

        /// <summary>
        /// Generates a new access token using a valid refresh token.
        /// </summary>
        /// <param name="dto">Refresh token request.</param>
        /// <returns>New access and refresh tokens.</returns>
        /// <response code="200">Token refreshed successfully.</response>
        /// <response code="400">Invalid or expired refresh token.</response>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenRequestDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto);

            if (result == null)
                return BadRequest(new { message = "Invalid or expired refresh token." });

            return Ok(result);
        }

        /// <summary>
        /// Logs out the current user by revoking the refresh token.
        /// </summary>
        /// <param name="dto">Refresh token to revoke.</param>
        /// <returns>Logout result.</returns>
        /// <response code="200">Logged out successfully.</response>
        /// <response code="400">Invalid refresh token.</response>
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
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Project_3.src.Application.Interfaces.IServices;
using Project_3.src.Application.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Project_3.src.Infrastructure.identity
{
    public class TokenService:ITokenService
    {
        private readonly JwtOptions _opt;

        public TokenService(IOptions<JwtOptions> opts)
        {
            _opt = opts.Value;
        }
        public string GenerateAccessToken(User user, IList<string> roles)
        {
            var claims = new List<Claim>()
           {
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim(ClaimTypes.Name,user.UserName ?? string.Empty),
               new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
           };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_opt.ExpiresInMinutes),
                signingCredentials: creds

                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}

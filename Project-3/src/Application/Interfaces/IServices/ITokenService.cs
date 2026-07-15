using Project_3.src.Application.Models;

namespace Project_3.src.Application.Interfaces.IServices
{
    public interface ITokenService
    {
        public string GenerateAccessToken(User user, IList<string> roles);
        string GenerateRefreshToken();

    }
}

using SocialNetwork.Core.Models.Auth;

namespace SocialNetwork.ApplicationLogic.Services.Auth
{
    public interface IAuthService
    {
        Task<(string? accessToken, RefreshToken? refreshToken, string error)> Login(string email, string password, string userAgent, string ipAddress);
        Task<(bool success, string error)> Logout(Guid userId, string userAgent);
        Task<(string? accessToken, RefreshToken? refreshToken, string error)> Register(string username, string email, string password, string userAgent, string ipAddress);
    }
}
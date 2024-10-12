using SocialNetwork.Core.Enums;
using SocialNetwork.Core.Models.Auth;

namespace SocialNetwork.ApplicationLogic.Services.Auth
{
    public interface ITokenService
    {
        string GenerateJwtAccessToken(Guid userId, UserRole role);
        Task<(string? AccessToken, RefreshToken? RefreshToken, string Error)> GenerateTokensAsync(Guid userId, string userAgent, string ipAddress, bool forceCreateNewRefreshToken = false);
        Task<(string? AccessToken, RefreshToken? RefreshToken, string Error)> RefreshAccessTokenAsync(string refreshToken, string userAgent, string ipAddress);
        Task<(bool Success, string Error)> RevokeAllTokensAsync(Guid userId);
        Task<(bool Success, string Error)> RevokeRefreshTokenByUserAgentAsync(Guid userId, string userAgent);
        Task<bool> ValidateRefreshTokenAsync(Guid userId, string token, string userAgent);
    }
}
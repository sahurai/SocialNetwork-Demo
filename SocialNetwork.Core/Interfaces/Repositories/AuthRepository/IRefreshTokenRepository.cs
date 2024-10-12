using SocialNetwork.Core.Models.Auth;

namespace SocialNetwork.DataAccess.Repository.Auth
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> CreateAsync(RefreshToken refreshToken);
        Task<Guid> DeleteAsync(Guid id);
        Task<List<RefreshToken>> GetAsync(Guid? refreshTokenId = null, Guid? userId = null, string? token = null, string? userAgent = null, string? ipAddress = null);
        Task<RefreshToken> UpdateAsync(RefreshToken updatedRefreshToken);
    }
}
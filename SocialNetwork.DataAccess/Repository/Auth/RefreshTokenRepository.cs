using Microsoft.EntityFrameworkCore;
using SocialNetwork.Core.Models.Auth;
using SocialNetwork.DataAccess.Entities.Auth;


namespace SocialNetwork.DataAccess.Repository.Auth
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly SocialNetworkDbContext _context;

        public RefreshTokenRepository(SocialNetworkDbContext context)
        {
            _context = context;
        }

        public async Task<List<RefreshToken>> GetAsync(Guid? refreshTokenId = null, Guid? userId = null, string? token = null, string? userAgent = null, string? ipAddress = null)
        {
            IQueryable<RefreshTokenEntity> query = _context.RefreshTokens.AsNoTracking();

            if (refreshTokenId.HasValue)
            {
                query = query.Where(rt => rt.Id == refreshTokenId.Value);
            }

            if (userId.HasValue)
            {
                query = query.Where(rt => rt.UserId == userId.Value);
            }

            if (!string.IsNullOrEmpty(token))
            {
                query = query.Where(rt => rt.Token == token);
            }

            if (!string.IsNullOrEmpty(userAgent))
            {
                query = query.Where(rt => rt.UserAgent == userAgent);
            }

            if (!string.IsNullOrEmpty(ipAddress))
            {
                query = query.Where(rt => rt.IpAddress == ipAddress);
            }

            List<RefreshTokenEntity> tokenEntities = await query.ToListAsync();

            List<RefreshToken> tokens = tokenEntities.Select(MapToModel).ToList();

            return tokens;
        }

        public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
        {
            // Creating entity
            RefreshTokenEntity tokenEntity = new RefreshTokenEntity
            {
                Token = refreshToken.Token,
                UserId = refreshToken.UserId,
                ExpiryDate = refreshToken.ExpiryDate,
                UserAgent = refreshToken.UserAgent,
                IpAddress = refreshToken.IpAddress,
            };

            // Adding to DB
            await _context.AddAsync(tokenEntity);
            await _context.SaveChangesAsync();

            return MapToModel(tokenEntity);
        }

        public async Task<RefreshToken> UpdateAsync(RefreshToken updatedRefreshToken)
        {
            await _context.RefreshTokens
                .Where(token => token.Id == updatedRefreshToken.Id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(token => token.Token, updatedRefreshToken.Token)
                    .SetProperty(token => token.ExpiryDate, updatedRefreshToken.ExpiryDate)
                    .SetProperty(token => token.UserAgent, updatedRefreshToken.UserAgent)
                    .SetProperty(token => token.IpAddress, updatedRefreshToken.IpAddress)
                    .SetProperty(token => token.UpdatedAt, updatedRefreshToken.UpdatedAt));

            return updatedRefreshToken;
        }

        public async Task<Guid> DeleteAsync(Guid id)
        {
            await _context.RefreshTokens
                  .Where(token => token.Id == id)
                  .ExecuteDeleteAsync();

            return id;
        }

        // Mapping
        private RefreshToken MapToModel(RefreshTokenEntity entity)
        {
            var (refreshToken, error) = RefreshToken.CreateFromDb(
                entity.Id,
                entity.Token,
                entity.UserId,
                entity.ExpiryDate,
                entity.UserAgent,
                entity.IpAddress,
                entity.CreatedAt,
                entity.UpdatedAt
            );

            if (refreshToken == null) throw new Exception($"Error during mapping: {error}");

            return refreshToken;
        }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Core.Enums;
using SocialNetwork.Core.Interfaces.Services.UserService;
using SocialNetwork.Core.Models.Auth;
using SocialNetwork.DataAccess.Repository.Auth;
using SocialNetwork.Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocialNetwork.ApplicationLogic.Services.Auth
{
    public class TokenService : ITokenService
    {
        private readonly IUserService _userService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GroupService> _logger;

        public TokenService(
            IUserService userService,
            IRefreshTokenRepository refreshTokenRepository,
            IConfiguration configuration,
            ILogger<GroupService> logger)
        {
            _userService = userService;
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
            _logger = logger;
        }

        // Generates JWT access token and refresh token for a user.
        public async Task<(string? AccessToken, RefreshToken? RefreshToken, string Error)> GenerateTokensAsync(
            Guid userId,
            string userAgent,
            string ipAddress,
            bool forceCreateNewRefreshToken = false)
        {
            try
            {
                // Retrieve user details
                var (users, error) = await _userService.GetUsersAsync(userId: userId);
                var user = users.FirstOrDefault();
                if (!string.IsNullOrEmpty(error) || user == null) return (null, null, error);

                // Retrieve existing refresh tokens
                var refreshTokens = await _refreshTokenRepository.GetAsync(userId: userId, userAgent: userAgent);
                var existingRefreshToken = refreshTokens.FirstOrDefault();

                // Reuse existing valid refresh token if not forcing a new one
                if (!forceCreateNewRefreshToken && existingRefreshToken != null && existingRefreshToken.ExpiryDate > DateTime.UtcNow)
                {
                    var accessToken = GenerateJwtAccessToken(userId, user.Role);
                    return (accessToken, existingRefreshToken, string.Empty);
                }

                // Delete existing refresh token to prevent duplicates
                if (existingRefreshToken != null)
                {
                    await _refreshTokenRepository.DeleteAsync(existingRefreshToken.Id);
                }

                // Generate new access token
                var newAccessToken = GenerateJwtAccessToken(userId, user.Role);

                // Create new refresh token
                var (newRefreshToken, createError) = RefreshToken.Create(
                    token: Guid.NewGuid().ToString(),
                    userId: userId,
                    expiryDate: DateTime.UtcNow.AddDays(Constants.RefreshTokenExpiresInDays),
                    userAgent: userAgent,
                    ipAddress: ipAddress
                );
                if (newRefreshToken == null) return (null, null, createError);

                // Save the new refresh token
                await _refreshTokenRepository.CreateAsync(newRefreshToken);

                return (newAccessToken, newRefreshToken, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating tokens.");
                return (null, null, "An error occurred while generating tokens.");
            }
        }

        // Generates a JWT access token for the user.
        public string GenerateJwtAccessToken(Guid userId, UserRole role)
        {
            try
            {
                var claims = new[]
                {
                    new Claim("userId", userId.ToString()),
                    new Claim(ClaimTypes.Role, role.ToString()),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(Constants.AccessTokenExpiresInMinutes),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating JWT access token.");
                throw new Exception($"An error occurred while generating JWT access token: {ex.Message}", ex);
            }
        }

        // Validates the refresh token for a user and user agent.
        public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string token, string userAgent)
        {
            try
            {
                var refreshTokens = await _refreshTokenRepository.GetAsync(userId: userId);
                var validToken = refreshTokens.FirstOrDefault(rt =>
                    rt.Token == token && rt.UserAgent == userAgent && rt.ExpiryDate > DateTime.UtcNow);

                return validToken != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while validating refresh token.");
                return false;
            }
        }

        // Refreshes the access token using a valid refresh token.
        public async Task<(string? AccessToken, RefreshToken? RefreshToken, string Error)> RefreshAccessTokenAsync(
            string refreshToken,
            string userAgent,
            string ipAddress)
        {
            try
            {
                // Retrieve the stored refresh token based on token string and user agent
                var storedRefreshTokens = await _refreshTokenRepository.GetAsync(token: refreshToken, userAgent: userAgent);
                var storedRefreshToken = storedRefreshTokens.FirstOrDefault();
                if (storedRefreshToken == null) return (null, null, "Invalid refresh token.");

                // If the token has expired, delete it and return an error
                if (storedRefreshToken.ExpiryDate <= DateTime.UtcNow)
                {
                    await _refreshTokenRepository.DeleteAsync(storedRefreshToken.Id);
                    return (null, null, "Refresh token has expired.");
                }

                // Determine if a new refresh token should be created (if less than one day remains)
                var timeRemaining = storedRefreshToken.ExpiryDate - DateTime.UtcNow;
                bool shouldCreateNewRefreshToken = timeRemaining < TimeSpan.FromDays(Constants.RefreshRefreshTokenInDays);

                if (shouldCreateNewRefreshToken)
                {
                    // Delete the old refresh token
                    await _refreshTokenRepository.DeleteAsync(storedRefreshToken.Id);

                    // Generate new tokens, forcing creation of a new refresh token
                    var (newAccessToken, newRefreshToken, generateError) = await GenerateTokensAsync(
                        storedRefreshToken.UserId,
                        userAgent,
                        ipAddress,
                        forceCreateNewRefreshToken: true
                    );
                    if (!string.IsNullOrEmpty(generateError)) return (null, null, generateError);

                    return (newAccessToken, newRefreshToken, string.Empty);
                }
                else
                {
                    // Generate a new access token without creating a new refresh token
                    var (users, error) = await _userService.GetUsersAsync(userId: storedRefreshToken.UserId);
                    var user = users.FirstOrDefault();
                    if (!string.IsNullOrEmpty(error) || user == null) return (null, null, "User not found.");

                    // Generate access token
                    var newAccessToken = GenerateJwtAccessToken(user.Id, user.Role);

                    // Return the new access token and existing refresh token
                    return (newAccessToken, storedRefreshToken, string.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while refreshing tokens.");
                return (null, null, "An error occurred while refreshing tokens.");
            }
        }

        // Revokes all refresh tokens for a user
        public async Task<(bool Success, string Error)> RevokeAllTokensAsync(Guid userId)
        {
            try
            {
                var refreshTokens = await _refreshTokenRepository.GetAsync(userId);

                foreach (var token in refreshTokens)
                {
                    await _refreshTokenRepository.DeleteAsync(token.Id);
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while revoking all refresh tokens.");
                return (false, "An error occurred while revoking all refresh tokens.");
            }
        }

        // Revokes a specific refresh token based on user agent.
        public async Task<(bool Success, string Error)> RevokeRefreshTokenByUserAgentAsync(Guid userId, string userAgent)
        {
            try
            {
                var refreshTokens = await _refreshTokenRepository.GetAsync(userId, null, userAgent);
                var refreshToken = refreshTokens.FirstOrDefault(rt => rt.UserAgent == userAgent);

                if (refreshToken != null)
                {
                    await _refreshTokenRepository.DeleteAsync(refreshToken.Id);
                    return (true, string.Empty);
                }

                return (false, "Refresh token not found for the specified user agent.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while revoking the refresh token.");
                return (false, "An error occurred while revoking the refresh token.");
            }
        }
    }
}

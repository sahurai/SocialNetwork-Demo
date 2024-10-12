using SocialNetwork.Core.Interfaces.Services.UserService;
using SocialNetwork.DataAccess.Repository.Auth;
using SocialNetwork.Core.Models.Auth;
using Microsoft.Extensions.Logging;
using SocialNetwork.Core.Models;
using SocialNetwork.Core.Enums;

namespace SocialNetwork.ApplicationLogic.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<GroupService> _logger;

        public AuthService(IUserService userService, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository, ILogger<GroupService> logger)
        {
            _userService = userService;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }

        // Registration
        public async Task<(string? accessToken, RefreshToken? refreshToken, string error)> Register(
            string username,
            string email,
            string password,
            string userAgent,
            string ipAddress
        )
        {
            try
            {
                // Check if user already exists
                var (existingUsers, error) = await _userService.GetUsersAsync(email: email);
                var existingUser = existingUsers.FirstOrDefault();
                if (existingUser != null) return (null, null, "User with this email already exists.");

                // Create new user
                var (user, createError) = await _userService.CreateUserAsync(username, email, UserRole.User, password);
                if (!string.IsNullOrEmpty(createError) || user == null) return (null, null, createError);

                // Generate tokens, allowing use of an existing refresh token
                var (accessToken, refreshToken, generateError) = await _tokenService.GenerateTokensAsync(user.Id, userAgent, ipAddress, false);
                if (!string.IsNullOrEmpty(generateError)) return (null, null, generateError);

                return (accessToken, refreshToken, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration.");
                return (null, null, $"An error occurred during registration.");
            }
        }

        // Login
        public async Task<(string? accessToken, RefreshToken? refreshToken, string error)> Login(string email, string password, string userAgent, string ipAddress)
        {
            try
            {
                // Get user from the service
                var (users, error) = await _userService.GetUsersAsync(email: email);
                var user = users.FirstOrDefault();
                if (user == null || !string.IsNullOrEmpty(error) || !IsPasswordValid(user, password)) return (null, null, "Invalid credentials");

                // Generate tokens, allowing use of an existing refresh token
                var (accessToken, refreshToken, generateError) = await _tokenService.GenerateTokensAsync(user.Id, userAgent, ipAddress, false);
                if (!string.IsNullOrEmpty(generateError)) return (null, null, generateError);

                return (accessToken, refreshToken, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return (null, null, $"An error occurred during login.");
            }
        }

        // Logout
        public async Task<(bool success, string error)> Logout(Guid userId, string userAgent)
        {
            try
            {
                // Getting refresh token
                var refreshTokens = await _refreshTokenRepository.GetAsync(userId: userId, userAgent: userAgent);
                var refreshToken = refreshTokens.FirstOrDefault();

                if (refreshToken != null)
                {
                    // Deleting refresh token
                    await _refreshTokenRepository.DeleteAsync(refreshToken.Id);
                    return (true, string.Empty);
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging out.");
                return (false, $"An error occurred while logging out.");
            }
        }

        // Verify password
        private bool IsPasswordValid(User user, string password)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while verifying the password: {ex.Message}", ex);
            }
        }
    }
}

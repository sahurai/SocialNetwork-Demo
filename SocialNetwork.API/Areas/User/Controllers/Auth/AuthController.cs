using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.API.DTO.User;
using SocialNetwork.ApplicationLogic.Services.Auth;
using SocialNetwork.Core.Models.Auth;
using SocialNetwork.Shared;

namespace SocialNetwork.API.Areas.User.Controllers.Auth
{
    [ApiController]
    [Area("User")]
    [Route("auth")]
    [ApiExplorerSettings(GroupName = "User")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;

        public AuthController(IAuthService authService, ITokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Get the user-agent from headers
            var userAgent = Request.Headers["User-Agent"].ToString();

            // Call the registration service method to register the user
            var (accessToken, refreshToken, error) = await _authService.Register(
                request.Username,
                request.Email,
                request.Password,
                userAgent,
                Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? ""
            );

            // If there's an error during registration, return a BadRequest with the error message
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { error });

            // Set the access and refresh tokens in the response cookies
            SetTokenCookies(accessToken, refreshToken);

            // Return the access token in the response body
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Get the user-agent from headers
            var userAgent = Request.Headers["User-Agent"].ToString();

            // Call the login service method to authenticate the user
            var (accessToken, refreshToken, error) = await _authService.Login(
                request.Email,
                request.Password,
                userAgent,
                Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? ""
            );

            // If there's an error during login, return a BadRequest with the error message
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { error });

            // Set the access and refresh tokens in the response cookies
            SetTokenCookies(accessToken, refreshToken);

            // Return the access token in the response body
            return Ok();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Get the user-agent from the request headers
            var userAgent = Request.Headers["User-Agent"].ToString();

            // Get user id from token
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            // Call the Logout method from AuthService
            var (isLoggedOut, error) = await _authService.Logout(userId.Value, userAgent);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { error });

            // Clear cookies
            ClearTokenCookies();

            // Return a successful response
            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            // Get the refresh token from cookies
            var refreshToken = Request.Cookies["RefreshToken"];
            // Get the user-agent from headers
            var userAgent = Request.Headers["X-Original-User-Agent"].ToString();

            // If the refresh token or User agent is missing, return a BadRequest
            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(userAgent))
            {
                ClearTokenCookies();
                return BadRequest(new { error = "Refresh token or User agent is missing." });
            }

            // Call the service method to refresh the access token
            var (newAccessToken, newRefreshToken, error) = await _tokenService.RefreshAccessTokenAsync(
                refreshToken,
                userAgent,
                Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? ""
            );

            // If there's an error during the refresh process, return a BadRequest with the error message
            if (!string.IsNullOrEmpty(error))
            {
                ClearTokenCookies();
                return BadRequest(new { error });
            }

            // Set the new access and refresh tokens in the response cookies
            SetTokenCookies(newAccessToken, newRefreshToken);

            // Return the new access token in the response body
            return Ok();
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> RevokeRefreshTokenByUserAgent([FromBody] RevokeTokenRequestByUserAgent request)
        {
            // Get user id from token
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            // Call the service method to revoke the specified refresh token
            var (success, errorMessage) = await _tokenService.RevokeRefreshTokenByUserAgentAsync(userId.Value, request.UserAgent);
            if (!success) return BadRequest(new { error = errorMessage });

            // If the token was successfully revoked, return an OK response
            return Ok();
        }

        // Helper method to set cookies for access and refresh tokens
        private void SetTokenCookies(string accessToken, RefreshToken refreshToken)
        {
            // Options for the access token cookie
            var accessTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true, // Ensures that the cookie is accessible only by the server-side code
                Secure = true,   // Ensures that the cookie is sent only over HTTPS
                SameSite = SameSiteMode.Strict, // Restricts the cookie to same-site requests only
                Expires = DateTime.UtcNow.AddMinutes(Constants.AccessTokenExpiresInMinutes) // Sets the cookie expiration for access token
            };

            // Options for the refresh token cookie
            var refreshTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshToken.ExpiryDate
            };

            // Append the access token and refresh token to the response cookies
            Response.Cookies.Append("AccessToken", accessToken, accessTokenCookieOptions);
            Response.Cookies.Append("RefreshToken", refreshToken.Token, refreshTokenCookieOptions);
        }

        // Helper method to clear the tokens from cookies
        private void ClearTokenCookies()
        {
            // Options for deleting the cookies
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1) // Set the cookie to expire in the past to delete it
            };

            // Remove tokens from cookies
            Response.Cookies.Append("AccessToken", string.Empty, cookieOptions);
            Response.Cookies.Append("RefreshToken", string.Empty, cookieOptions);
        }
    }
}

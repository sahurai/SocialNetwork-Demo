using Microsoft.AspNetCore.Mvc;
using System.Data;
using SocialNetwork.Core.Interfaces.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using SocialNetwork.API.DTO.User;
using SocialNetwork.Shared;

namespace SocialNetwork.API.Areas.User.Controllers.User
{
    [ApiController]
    [Authorize]
    [Area("User")]
    [Route("users")]
    [ApiExplorerSettings(GroupName = "User")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: users?userId=GUID&username=string
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] Guid? userId, [FromQuery] string? username)
        {
            var (users, error) = await _userService.GetUsersAsync(userId, username);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Error = error });

            var response = users.Select(user => new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                CreatedAt = user.CreatedAt,
            }).ToList();

            return Ok(response);
        }

        // PUT: users
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (updatedUser, error) = await _userService.UpdateUserAsync(userId.Value, request.Username, request.Email, request.Password);
            if (!string.IsNullOrEmpty(error) || updatedUser == null) return BadRequest(new { Error = error });

            var response = new UserResponse
            {
                Id = updatedUser.Id,
                Username = updatedUser.Username,
                CreatedAt = updatedUser.CreatedAt
            };

            return Ok(response);
        }

        // DELETE: users
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser()
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (deletedId, error) = await _userService.DeleteUserAsync(userId.Value);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

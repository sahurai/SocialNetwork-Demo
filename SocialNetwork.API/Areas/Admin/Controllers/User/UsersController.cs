using Microsoft.AspNetCore.Mvc;
using System.Data;
using SocialNetwork.Core.Interfaces.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using SocialNetwork.Core.Enums;
using SocialNetwork.API.DTO.User;
using SocialNetwork.Core.Models;

namespace SocialNetwork.API.Areas.Admin.Controllers.User
{
    [ApiController]
    [Area("Admin")]
    [Route("admin/users")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: admin/users?userId=GUID&username=string&email=string&role=int
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] Guid? userId, [FromQuery] string? username, [FromQuery] string? email, [FromQuery] UserRole? role)
        {
            var (users, error) = await _userService.GetUsersAsync(userId, username, email, role);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Error = error });

            var response = users.Select(user => new UserResponseWithAdditionalInfo
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                Password = user.PasswordHash,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }).ToList();

            return Ok(response);
        }

        // POST: admin/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var (user, error) = await _userService.CreateUserAsync(request.Username, request.Email, request.Role, request.Password);
            if (!string.IsNullOrEmpty(error) || user == null) return BadRequest(new { Error = error });

            var response = new UserResponseWithAdditionalInfo
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                Password = user.PasswordHash,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return CreatedAtAction(nameof(GetUsers), new { userId = user.Id }, response);
        }

        // PUT: admin/users/{userId}
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
        {
            var (updatedUser, error) = await _userService.UpdateUserAsync(userId, request.Username, request.Email, request.Password);
            if (!string.IsNullOrEmpty(error) || updatedUser == null) return BadRequest(new { Error = error });

            var response = new UserResponseWithAdditionalInfo
            {
                Id = updatedUser.Id,
                Username = updatedUser.Username,
                Role = updatedUser.Role,
                Password = updatedUser.PasswordHash,
                Email = updatedUser.Email,
                CreatedAt = updatedUser.CreatedAt,
                UpdatedAt = updatedUser.UpdatedAt
            };

            return Ok(response);
        }

        // DELETE: admin/users/{userId}
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var (deletedId, error) = await _userService.DeleteUserAsync(userId);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

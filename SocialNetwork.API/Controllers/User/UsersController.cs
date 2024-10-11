using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Api.DTOs;
using System.Data;
using SocialNetwork.Core.Interfaces.Services.UserService;

namespace SocialNetwork.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: api/users?userId=GUID&username=string&email=string
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] Guid? userId, [FromQuery] string? username, [FromQuery] string? email)
        {
            var (users, error) = await _userService.GetUsersAsync(userId, username, email);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Error = error });

            var response = users.Select(user => new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }).ToList();

            return Ok(response);
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var (user, error) = await _userService.CreateUserAsync(request.Username, request.Email, request.Password);
            if (!string.IsNullOrEmpty(error) || user == null) return BadRequest(new { Error = error });

            var response = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return CreatedAtAction(nameof(GetUsers), new { userId = user.Id }, response);
        }

        // PUT: api/users/{userId}
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
        {
            var (updatedUser, error) = await _userService.UpdateUserAsync(userId, request.Username, request.Email, request.Password);
            if (!string.IsNullOrEmpty(error) || updatedUser == null) return BadRequest(new { Error = error });

            var response = new UserResponse
            {
                Id = updatedUser.Id,
                Username = updatedUser.Username,
                Email = updatedUser.Email,
                CreatedAt = updatedUser.CreatedAt,
                UpdatedAt = updatedUser.UpdatedAt
            };

            return Ok(response);
        }

        // DELETE: api/users/{userId}?requestingUserId=GUID
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId, [FromQuery] Guid requestingUserId)
        {
            var (deletedId, error) = await _userService.DeleteUserAsync(userId, requestingUserId);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

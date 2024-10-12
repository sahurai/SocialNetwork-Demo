using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using System.Data;
using SocialNetwork.API.DTO.User;
using Microsoft.AspNetCore.Authorization;
using SocialNetwork.Shared;

namespace SocialNetwork.API.Areas.User.Controllers.User
{
    [ApiController]
    [Authorize]
    [Area("User")]
    [Route("friendships")]
    [ApiExplorerSettings(GroupName = "User")]
    public class FriendshipsController : ControllerBase
    {
        private readonly IFriendshipService _friendshipService;
        private readonly ILogger<FriendshipsController> _logger;

        public FriendshipsController(IFriendshipService friendshipService, ILogger<FriendshipsController> logger)
        {
            _friendshipService = friendshipService;
            _logger = logger;
        }

        // GET: friendships
        [HttpGet]
        public async Task<IActionResult> GetFriendships()
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (friendships, error) = await _friendshipService.GetFriendshipsAsync(userId.Value);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Error = error });

            var response = friendships.Select(friendship => new FriendshipResponse
            {
                Id = friendship.Id,
                User1Id = friendship.User1Id,
                User2Id = friendship.User2Id,
                IsAccepted = friendship.AcceptedAt != null,
                CreatedAt = friendship.CreatedAt,
                UpdatedAt = friendship.UpdatedAt
            }).ToList();

            return Ok(response);
        }

        // POST: friendships
        [HttpPost]
        public async Task<IActionResult> CreateFriendship([FromBody] CreateFriendshipByUserRequest request)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (friendship, error) = await _friendshipService.CreateFriendshipAsync(userId.Value, request.newFriendId);
            if (!string.IsNullOrEmpty(error) || friendship == null) return BadRequest(new { Error = error });

            var response = new FriendshipResponse
            {
                Id = friendship.Id,
                User1Id = friendship.User1Id,
                User2Id = friendship.User2Id,
                IsAccepted = friendship.AcceptedAt != null,
                CreatedAt = friendship.CreatedAt,
                UpdatedAt = friendship.UpdatedAt
            };

            return CreatedAtAction(nameof(GetFriendships), new { userId = friendship.User1Id }, response);
        }

        // PUT: friendships/{friendshipId}/accept
        [HttpPut("{friendshipId}/accept")]
        public async Task<IActionResult> AcceptFriendship(Guid friendshipId)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (friendship, error) = await _friendshipService.AcceptFriendshipAsync(friendshipId, userId.Value);
            if (!string.IsNullOrEmpty(error) || friendship == null) return BadRequest(new { Error = error });

            var response = new FriendshipResponse
            {
                Id = friendship.Id,
                User1Id = friendship.User1Id,
                User2Id = friendship.User2Id,
                IsAccepted = friendship.AcceptedAt != null,
                CreatedAt = friendship.CreatedAt,
                UpdatedAt = friendship.UpdatedAt
            };

            return Ok(response);
        }

        // DELETE: friendships/{friendshipId}
        [HttpDelete("{friendshipId}")]
        public async Task<IActionResult> DeleteFriendship(Guid friendshipId)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (deletedId, error) = await _friendshipService.DeleteFriendshipAsync(friendshipId, userId.Value);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

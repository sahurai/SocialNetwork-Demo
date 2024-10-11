using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using SocialNetwork.Api.DTOs;
using System.Data;

namespace SocialNetwork.Api.Controllers
{
    [ApiController]
    [Route("api/friendships")]
    public class FriendshipsController : ControllerBase
    {
        private readonly IFriendshipService _friendshipService;
        private readonly ILogger<FriendshipsController> _logger;

        public FriendshipsController(IFriendshipService friendshipService, ILogger<FriendshipsController> logger)
        {
            _friendshipService = friendshipService;
            _logger = logger;
        }

        // GET: api/friendships?userId=GUID
        [HttpGet]
        public async Task<IActionResult> GetFriendships([FromQuery] Guid userId)
        {
            var (friendships, error) = await _friendshipService.GetFriendshipsAsync(userId);
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

        // POST: api/friendships
        [HttpPost]
        public async Task<IActionResult> CreateFriendship([FromBody] CreateFriendshipRequest request)
        {
            var (friendship, error) = await _friendshipService.CreateFriendshipAsync(request.RequestingUserId, request.User2Id);
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

        // PUT: api/friendships/{friendshipId}/accept
        [HttpPut("{friendshipId}/accept")]
        public async Task<IActionResult> AcceptFriendship(Guid friendshipId, [FromQuery] Guid userId)
        {
            var (friendship, error) = await _friendshipService.AcceptFriendshipAsync(friendshipId, userId);
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

        // DELETE: api/friendships/{friendshipId}?requestingUserId=GUID
        [HttpDelete("{friendshipId}")]
        public async Task<IActionResult> DeleteFriendship(Guid friendshipId, [FromQuery] Guid requestingUserId)
        {
            var (deletedId, error) = await _friendshipService.DeleteFriendshipAsync(friendshipId, requestingUserId);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

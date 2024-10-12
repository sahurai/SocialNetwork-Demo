using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using SocialNetwork.Core.Enums;
using SocialNetwork.API.DTO.User;

namespace SocialNetwork.API.Areas.Admin.Controllers.User
{
    [ApiController]
    [Area("Admin")]
    [Route("admin/friendships")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class FriendshipsController : ControllerBase
    {
        private readonly IFriendshipService _friendshipService;
        private readonly ILogger<FriendshipsController> _logger;

        public FriendshipsController(IFriendshipService friendshipService, ILogger<FriendshipsController> logger)
        {
            _friendshipService = friendshipService;
            _logger = logger;
        }

        // GET: admin/friendships?userId=GUID
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

        // POST: admin/friendships
        [HttpPost]
        public async Task<IActionResult> CreateFriendship([FromBody] CreateFriendshipRequest request)
        {
            var (friendship, error) = await _friendshipService.CreateFriendshipAsync(request.User1Id, request.User2Id);
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

        // PUT: admin/friendships/{friendshipId}/accept
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

        // DELETE: admin/friendships/{friendshipId}?userIdInFriendship=GUID
        [HttpDelete("{friendshipId}")]
        public async Task<IActionResult> DeleteFriendship(Guid friendshipId, [FromQuery] Guid userIdInFriendship)
        {
            var (deletedId, error) = await _friendshipService.DeleteFriendshipAsync(friendshipId, userIdInFriendship);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

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
    [Route("user-blocks")]
    [ApiExplorerSettings(GroupName = "User")]
    public class UserBlocksController : ControllerBase
    {
        private readonly IUserBlockService _userBlockService;
        private readonly ILogger<UserBlocksController> _logger;

        public UserBlocksController(IUserBlockService userBlockService, ILogger<UserBlocksController> logger)
        {
            _userBlockService = userBlockService;
            _logger = logger;
        }

        // GET: user-blocks
        [HttpGet]
        public async Task<IActionResult> GetUserBlocks()
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (blocks, error) = await _userBlockService.GetUserBlocksAsync(userId.Value);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Error = error });

            var response = blocks.Select(block => new UserBlockResponse
            {
                Id = block.Id,
                BlockerId = block.BlockerId,
                BlockedId = block.BlockedId,
                CreatedAt = block.CreatedAt,
                UpdatedAt = block.UpdatedAt
            }).ToList();

            return Ok(response);
        }

        // POST: user-blocks
        [HttpPost]
        public async Task<IActionResult> CreateUserBlock([FromBody] CreateUserBlockByUserRequest request)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (block, error) = await _userBlockService.CreateUserBlockAsync(userId.Value, request.BlockedId);
            if (!string.IsNullOrEmpty(error) || block == null) return BadRequest(new { Error = error });

            var response = new UserBlockResponse
            {
                Id = block.Id,
                BlockerId = block.BlockerId,
                BlockedId = block.BlockedId,
                CreatedAt = block.CreatedAt,
                UpdatedAt = block.UpdatedAt
            };

            return CreatedAtAction(nameof(GetUserBlocks), new { userBlockId = block.Id }, response);
        }

        // DELETE: user-blocks/{userBlockId}
        [HttpDelete("{userBlockId}")]
        public async Task<IActionResult> DeleteUserBlock(Guid userBlockId)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (deletedId, error) = await _userBlockService.DeleteUserBlockAsync(userBlockId, userId.Value);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

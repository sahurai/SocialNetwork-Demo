using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using SocialNetwork.Api.DTOs;
using System.Data;

namespace SocialNetwork.Api.Controllers
{
    [ApiController]
    [Route("api/user/blocks")]
    public class UserBlocksController : ControllerBase
    {
        private readonly IUserBlockService _userBlockService;
        private readonly ILogger<UserBlocksController> _logger;

        public UserBlocksController(IUserBlockService userBlockService, ILogger<UserBlocksController> logger)
        {
            _userBlockService = userBlockService;
            _logger = logger;
        }

        // GET: api/user/blocks?blockerId=GUID&blockedId=GUID
        [HttpGet]
        public async Task<IActionResult> GetUserBlocks([FromQuery] Guid? blockerId, [FromQuery] Guid? blockedId)
        {
            var (blocks, error) = await _userBlockService.GetUserBlocksAsync(blockerId, blockedId);
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

        // POST: api/user/blocks
        [HttpPost]
        public async Task<IActionResult> CreateUserBlock([FromBody] CreateUserBlockRequest request)
        {
            var (block, error) = await _userBlockService.CreateUserBlockAsync(request.RequestingUserId, request.BlockedId);
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

        // DELETE: api/user/blocks/{userBlockId}?requestingUserId=GUID
        [HttpDelete("{userBlockId}")]
        public async Task<IActionResult> DeleteUserBlock(Guid userBlockId, [FromQuery] Guid requestingUserId)
        {
            var (deletedId, error) = await _userBlockService.DeleteUserBlockAsync(userBlockId, requestingUserId);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

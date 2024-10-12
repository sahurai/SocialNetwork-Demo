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
    [Route("admin/user-blocks")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class UserBlocksController : ControllerBase
    {
        private readonly IUserBlockService _userBlockService;
        private readonly ILogger<UserBlocksController> _logger;

        public UserBlocksController(IUserBlockService userBlockService, ILogger<UserBlocksController> logger)
        {
            _userBlockService = userBlockService;
            _logger = logger;
        }

        // GET: admin/user-blocks?blockerId=GUID&blockedId=GUID
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

        // POST: admin/user-blocks
        [HttpPost]
        public async Task<IActionResult> CreateUserBlock([FromBody] CreateUserBlockRequest request)
        {
            var (block, error) = await _userBlockService.CreateUserBlockAsync(request.BlockerId, request.BlockedId);
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

        // DELETE: admin/user-blocks/{userBlockId}
        [HttpDelete("{userBlockId}")]
        public async Task<IActionResult> DeleteUserBlock(Guid userBlockId)
        {
            var (deletedId, error) = await _userBlockService.DeleteUserBlockAsync(userBlockId, userBlockId);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

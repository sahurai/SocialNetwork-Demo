using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using SocialNetwork.Core.Enums;
using SocialNetwork.API.DTO.Group;
using SocialNetwork.Shared;

namespace SocialNetwork.API.Areas.User.Controllers.Group
{
    [ApiController]
    [Authorize]
    [Area("User")]
    [Route("group-blocks")]
    [ApiExplorerSettings(GroupName = "User")]
    public class GroupBlocksController : ControllerBase
    {
        private readonly IGroupBlockService _groupBlockService;
        private readonly ILogger<GroupBlocksController> _logger;

        public GroupBlocksController(IGroupBlockService groupBlockService, ILogger<GroupBlocksController> logger)
        {
            _groupBlockService = groupBlockService;
            _logger = logger;
        }

        // GET: groups-blocks
        [HttpGet]
        public async Task<IActionResult> GetGroupBlocks([FromQuery] Guid groupId, [FromQuery] Guid? groupBlockId, [FromQuery] Guid? blockerId, [FromQuery] Guid? blockedId)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (blocks, error) = await _groupBlockService.GetGroupBlocksAsync(userId.Value, groupId, groupBlockId, blockerId, blockedId);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Error = error });

            var response = blocks.Select(block => new GroupBlockResponse
            {
                Id = block.Id,
                GroupId = block.GroupId,
                BlockerId = block.BlockerId,
                BlockedId = block.BlockedId,
                CreatedAt = block.CreatedAt,
                UpdatedAt = block.UpdatedAt
            }).ToList();

            return Ok(response);
        }

        // POST: group-blocks
        [HttpPost]
        public async Task<IActionResult> CreateGroupBlock([FromBody] CreateGroupBlockByUserRequest request)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (block, error) = await _groupBlockService.CreateGroupBlockAsync(request.GroupId, userId.Value, request.BlockedId);
            if (!string.IsNullOrEmpty(error) || block == null) return BadRequest(new { Error = error });

            var response = new GroupBlockResponse
            {
                Id = block.Id,
                GroupId = block.GroupId,
                BlockerId = block.BlockerId,
                BlockedId = block.BlockedId,
                CreatedAt = block.CreatedAt,
                UpdatedAt = block.UpdatedAt
            };

            return CreatedAtAction(nameof(GetGroupBlocks), new { groupBlockId = block.Id }, response);
        }

        // DELETE: groups-blocks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroupBlock(Guid id)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (deletedId, error) = await _groupBlockService.DeleteGroupBlockAsync(id, userId.Value);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using SocialNetwork.Core.Enums;
using SocialNetwork.API.DTO.Group;

namespace SocialNetwork.API.Areas.Admin.Controllers.Group
{
    [ApiController]
    [Area("Admin")]
    [Route("admin/group-blocks")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class GroupBlocksController : ControllerBase
    {
        private readonly IGroupBlockService _groupBlockService;
        private readonly ILogger<GroupBlocksController> _logger;

        public GroupBlocksController(IGroupBlockService groupBlockService, ILogger<GroupBlocksController> logger)
        {
            _groupBlockService = groupBlockService;
            _logger = logger;
        }

        // GET: admin/group-blocks
        [HttpGet]
        public async Task<IActionResult> GetGroupBlocks([FromQuery] Guid userId, [FromQuery] Guid groupId, [FromQuery] Guid? groupBlockId,  [FromQuery] Guid? blockerId, [FromQuery] Guid? blockedId)
        {
            var (blocks, error) = await _groupBlockService.GetGroupBlocksAsync(userId, groupId, groupBlockId, blockerId, blockedId);
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

        // POST: admin/group-blocks
        [HttpPost]
        public async Task<IActionResult> CreateGroupBlock([FromBody] CreateGroupBlockRequest request)
        {
            var (block, error) = await _groupBlockService.CreateGroupBlockAsync(request.GroupId, request.UserId, request.BlockedId);
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

        // DELETE: admin/group-blocks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroupBlock(Guid id, [FromQuery] Guid blockerId)
        {
            var (deletedId, error) = await _groupBlockService.DeleteGroupBlockAsync(id, blockerId);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

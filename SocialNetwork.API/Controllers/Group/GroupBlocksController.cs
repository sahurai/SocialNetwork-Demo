using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using SocialNetwork.Api.DTOs;
using System.Data;

namespace SocialNetwork.Api.Controllers
{
    [ApiController]
    [Route("api/groups/blocks")]
    public class GroupBlocksController : ControllerBase
    {
        private readonly IGroupBlockService _groupBlockService;
        private readonly ILogger<GroupBlocksController> _logger;

        public GroupBlocksController(IGroupBlockService groupBlockService, ILogger<GroupBlocksController> logger)
        {
            _groupBlockService = groupBlockService;
            _logger = logger;
        }

        // GET: api/groups/blocks
        [HttpGet]
        public async Task<IActionResult> GetGroupBlocks([FromQuery] Guid? groupBlockId, [FromQuery] Guid? groupId, [FromQuery] Guid? blockerId, [FromQuery] Guid? blockedId)
        {
            var (blocks, error) = await _groupBlockService.GetGroupBlocksAsync(groupBlockId, groupId, blockerId, blockedId);
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

        // POST: api/groups/blocks
        [HttpPost]
        public async Task<IActionResult> CreateGroupBlock([FromBody] CreateGroupBlockRequest request)
        {
            var (block, error) = await _groupBlockService.CreateGroupBlockAsync(request.GroupId, request.RequestingUserId, request.BlockedId);
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

        // DELETE: api/groups/blocks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroupBlock(Guid id, [FromQuery] Guid requestingUserId)
        {
            var (deletedId, error) = await _groupBlockService.DeleteGroupBlockAsync(id, requestingUserId);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

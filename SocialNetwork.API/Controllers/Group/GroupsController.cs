using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using SocialNetwork.Api.DTOs;
using System.Data;

namespace SocialNetwork.Api.Controllers
{
    [ApiController]
    [Route("api/groups")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly ILogger<GroupsController> _logger;

        public GroupsController(IGroupService groupService, ILogger<GroupsController> logger)
        {
            _groupService = groupService;
            _logger = logger;
        }

        // GET: api/groups
        [HttpGet]
        public async Task<IActionResult> GetGroups([FromQuery] Guid? groupId, [FromQuery] Guid? creatorId, [FromQuery] string? name, [FromQuery] string? description)
        {
            var (groups, error) = await _groupService.GetGroupsAsync(groupId, creatorId, name, description);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Error = error });

            var response = groups.Select(group => new GroupResponse
            {
                Id = group.Id,
                CreatorId = group.CreatorId,
                Name = group.Name,
                Description = group.Description,
                CreatedAt = group.CreatedAt,
                UpdatedAt = group.UpdatedAt
            }).ToList();

            return Ok(response);
        }

        // POST: api/groups
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        {
            var (group, error) = await _groupService.CreateGroupAsync(request.RequestingUserId, request.Name, request.Description);
            if (!string.IsNullOrEmpty(error) || group == null) return BadRequest(new { Error = error });

            var response = new GroupResponse
            {
                Id = group.Id,
                CreatorId = group.CreatorId,
                Name = group.Name,
                Description = group.Description,
                CreatedAt = group.CreatedAt,
                UpdatedAt = group.UpdatedAt
            };

            return CreatedAtAction(nameof(GetGroups), new { groupId = group.Id }, response);
        }

        // PUT: api/groups/{groupId}
        [HttpPut("{groupId}")]
        public async Task<IActionResult> UpdateGroup(Guid groupId, [FromBody] UpdateGroupRequest request)
        {
            var (updatedGroup, error) = await _groupService.UpdateGroupAsync(groupId, request.RequestingUserId, request.CreatorId, request.Name, request.Description);
            if (!string.IsNullOrEmpty(error) || updatedGroup == null) return BadRequest(new { Error = error });

            var response = new GroupResponse
            {
                Id = updatedGroup.Id,
                CreatorId = updatedGroup.CreatorId,
                Name = updatedGroup.Name,
                Description = updatedGroup.Description,
                CreatedAt = updatedGroup.CreatedAt,
                UpdatedAt = updatedGroup.UpdatedAt
            };

            return Ok(response);
        }

        // DELETE: api/groups/{groupId}
        [HttpDelete("{groupId}")]
        public async Task<IActionResult> DeleteGroup(Guid groupId, [FromQuery] Guid requestingUserId)
        {
            var (deletedId, error) = await _groupService.DeleteGroupAsync(groupId, requestingUserId);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

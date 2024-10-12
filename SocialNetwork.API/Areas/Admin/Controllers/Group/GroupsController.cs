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
    [Route("admin/groups")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly ILogger<GroupsController> _logger;

        public GroupsController(IGroupService groupService, ILogger<GroupsController> logger)
        {
            _groupService = groupService;
            _logger = logger;
        }

        // GET: admin/groups
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

        // POST: admin/groups
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        {
            var (group, error) = await _groupService.CreateGroupAsync(request.UserId, request.Name, request.Description);
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

        // PUT: admin/groups/{groupId}
        [HttpPut("{groupId}")]
        public async Task<IActionResult> UpdateGroup(Guid groupId, [FromBody] UpdateGroupRequest request)
        {
            var (updatedGroup, error) = await _groupService.UpdateGroupAsync(groupId, request.UserId, request.Name, request.Description);
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

        // DELETE: admin/groups/{groupId}
        [HttpDelete("{groupId}")]
        public async Task<IActionResult> DeleteGroup(Guid groupId, [FromQuery] Guid creatorId)
        {
            var (deletedId, error) = await _groupService.DeleteGroupAsync(groupId, creatorId);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using System.Data;
using SocialNetwork.API.DTO.Group;
using Microsoft.AspNetCore.Authorization;
using SocialNetwork.Shared;

namespace SocialNetwork.API.Areas.User.Controllers.Group
{
    [ApiController]
    [Authorize]
    [Area("User")]
    [Route("groups")]
    [ApiExplorerSettings(GroupName = "User")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly ILogger<GroupsController> _logger;

        public GroupsController(IGroupService groupService, ILogger<GroupsController> logger)
        {
            _groupService = groupService;
            _logger = logger;
        }

        // GET: groups
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

        // POST: groups
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupByUserRequest request)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (group, error) = await _groupService.CreateGroupAsync(userId.Value, request.Name, request.Description);
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

        // PUT: groups/{groupId}
        [HttpPut("{groupId}")]
        public async Task<IActionResult> UpdateGroup(Guid groupId, [FromBody] UpdateGroupByUserRequest request)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (updatedGroup, error) = await _groupService.UpdateGroupAsync(groupId, userId.Value, request.Name, request.Description);
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

        // DELETE: groups/{groupId}
        [HttpDelete("{groupId}")]
        public async Task<IActionResult> DeleteGroup(Guid groupId)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (deletedId, error) = await _groupService.DeleteGroupAsync(groupId, userId.Value);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

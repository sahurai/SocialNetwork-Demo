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
    [Route("group-roles")]
    [ApiExplorerSettings(GroupName = "User")]
    public class GroupUserRolesController : ControllerBase
    {
        private readonly IGroupUserRoleService _groupUserRoleService;
        private readonly ILogger<GroupUserRolesController> _logger;

        public GroupUserRolesController(IGroupUserRoleService groupUserRoleService, ILogger<GroupUserRolesController> logger)
        {
            _groupUserRoleService = groupUserRoleService;
            _logger = logger;
        }

        // GET: group-roles
        [HttpGet]
        public async Task<IActionResult> GetGroupUserRoles([FromQuery] Guid groupId, [FromQuery] Guid? groupUserRoleId, [FromQuery] Guid? memberId)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (roles, error) = await _groupUserRoleService.GetGroupUserRolesAsync(userId.Value, groupId, groupUserRoleId, memberId);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Error = error });

            return Ok(roles);
        }

        // POST: group-roles
        [HttpPost]
        public async Task<IActionResult> CreateGroupUserRole([FromBody] CreateGroupUserRoleByUserRequest request)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (role, error) = await _groupUserRoleService.CreateGroupUserRoleAsync(request.GroupId, userId.Value);
            if (!string.IsNullOrEmpty(error) || role == null) return BadRequest(new { Error = error });

            return CreatedAtAction(nameof(GetGroupUserRoles), new { groupUserRoleId = role.Id }, role);
        }

        // PUT: group-roles/{groupUserRoleId}
        [HttpPut("{groupUserRoleId}")]
        public async Task<IActionResult> UpdateGroupUserRole(Guid groupUserRoleId, [FromBody] UpdateGroupUserRoleRequest request)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (updatedRole, error) = await _groupUserRoleService.UpdateGroupUserRoleAsync(groupUserRoleId, userId.Value, request.Role);
            if (!string.IsNullOrEmpty(error) || updatedRole == null) return BadRequest(new { Error = error });

            return Ok(updatedRole);
        }

        // DELETE: group-roles/{groupUserRoleId}
        [HttpDelete("{groupUserRoleId}")]
        public async Task<IActionResult> DeleteGroupUserRole(Guid groupUserRoleId)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (deletedId, error) = await _groupUserRoleService.DeleteGroupUserRoleAsync(groupUserRoleId, userId.Value);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using Microsoft.AspNetCore.Authorization;
using SocialNetwork.Core.Enums;
using SocialNetwork.API.DTO.Group;

namespace SocialNetwork.API.Areas.Admin.Controllers.Group
{
    [ApiController]
    [Area("Admin")]
    [Route("admin/group-roles")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class GroupUserRolesController : ControllerBase
    {
        private readonly IGroupUserRoleService _groupUserRoleService;
        private readonly ILogger<GroupUserRolesController> _logger;

        public GroupUserRolesController(IGroupUserRoleService groupUserRoleService, ILogger<GroupUserRolesController> logger)
        {
            _groupUserRoleService = groupUserRoleService;
            _logger = logger;
        }

        // GET: admin/group-roles
        [HttpGet]
        public async Task<IActionResult> GetGroupUserRoles([FromQuery] Guid userId, [FromQuery] Guid groupId, [FromQuery] Guid? groupUserRoleId,  [FromQuery] Guid? memberId)
        {
            var (roles, error) = await _groupUserRoleService.GetGroupUserRolesAsync(userId, groupId, groupUserRoleId, memberId);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Error = error });

            return Ok(roles);
        }

        // POST: admin/group-roles
        [HttpPost]
        public async Task<IActionResult> CreateGroupUserRole([FromBody] CreateGroupUserRoleRequest request)
        {
            var (role, error) = await _groupUserRoleService.CreateGroupUserRoleAsync(request.GroupId, request.UserId);
            if (!string.IsNullOrEmpty(error) || role == null) return BadRequest(new { Error = error });

            return CreatedAtAction(nameof(GetGroupUserRoles), new { groupUserRoleId = role.Id }, role);
        }

        // PUT: admin/group-roles/{groupUserRoleId}
        [HttpPut("{groupUserRoleId}")]
        public async Task<IActionResult> UpdateGroupUserRole(Guid groupUserRoleId, [FromBody] UpdateGroupUserRoleRequest request)
        {
            var (updatedRole, error) = await _groupUserRoleService.UpdateGroupUserRoleAsync(groupUserRoleId, request.UserId, request.Role);
            if (!string.IsNullOrEmpty(error) || updatedRole == null) return BadRequest(new { Error = error });

            return Ok(updatedRole);
        }

        // DELETE: admin/group-roles/{groupUserRoleId}
        [HttpDelete("{groupUserRoleId}")]
        public async Task<IActionResult> DeleteGroupUserRole(Guid groupUserRoleId, [FromQuery] Guid userWithRightsId)
        {
            var (deletedId, error) = await _groupUserRoleService.DeleteGroupUserRoleAsync(groupUserRoleId, userWithRightsId);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

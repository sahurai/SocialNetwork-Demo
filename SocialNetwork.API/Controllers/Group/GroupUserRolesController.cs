using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using SocialNetwork.Api.DTOs;
using System.Data;

namespace SocialNetwork.Api.Controllers
{
    [ApiController]
    [Route("api/groups/roles")]
    public class GroupUserRolesController : ControllerBase
    {
        private readonly IGroupUserRoleService _groupUserRoleService;
        private readonly ILogger<GroupUserRolesController> _logger;

        public GroupUserRolesController(IGroupUserRoleService groupUserRoleService, ILogger<GroupUserRolesController> logger)
        {
            _groupUserRoleService = groupUserRoleService;
            _logger = logger;
        }

        // GET: api/groups/roles
        [HttpGet]
        public async Task<IActionResult> GetGroupUserRoles([FromQuery] Guid? groupUserRoleId, [FromQuery] Guid? groupId, [FromQuery] Guid? userId)
        {
            var (roles, error) = await _groupUserRoleService.GetGroupUserRolesAsync(groupUserRoleId, groupId, userId);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Error = error });

            return Ok(roles);
        }

        // POST: api/groups/roles
        [HttpPost]
        public async Task<IActionResult> CreateGroupUserRole([FromBody] CreateGroupUserRoleRequest request)
        {
            var (role, error) = await _groupUserRoleService.CreateGroupUserRoleAsync(request.GroupId, request.RequestingUserId, request.Role);
            if (!string.IsNullOrEmpty(error) || role == null) return BadRequest(new { Error = error });

            return CreatedAtAction(nameof(GetGroupUserRoles), new { groupUserRoleId = role.Id }, role);
        }

        // PUT: api/groups/roles/{groupUserRoleId}
        [HttpPut("{groupUserRoleId}")]
        public async Task<IActionResult> UpdateGroupUserRole(Guid groupUserRoleId, [FromBody] UpdateGroupUserRoleRequest request)
        {
            var (updatedRole, error) = await _groupUserRoleService.UpdateGroupUserRoleAsync(groupUserRoleId, request.RequestingUserId, request.Role);
            if (!string.IsNullOrEmpty(error) || updatedRole == null) return BadRequest(new { Error = error });

            return Ok(updatedRole);
        }

        // DELETE: api/groups/roles/{groupUserRoleId}
        [HttpDelete("{groupUserRoleId}")]
        public async Task<IActionResult> DeleteGroupUserRole(Guid groupUserRoleId, [FromQuery] Guid requestingUserId)
        {
            var (deletedId, error) = await _groupUserRoleService.DeleteGroupUserRoleAsync(groupUserRoleId, requestingUserId);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

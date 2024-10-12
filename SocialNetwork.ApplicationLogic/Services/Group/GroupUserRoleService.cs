using Microsoft.Extensions.Logging;
using SocialNetwork.Core.Enums;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Repositories;

namespace SocialNetwork.ApplicationLogic.Services
{
    public class GroupUserRoleService : IGroupUserRoleService
    {
        private readonly IGroupUserRoleRepository _roleRepository;
        private readonly ILogger<GroupUserRoleService> _logger;

        public GroupUserRoleService(IGroupUserRoleRepository roleRepository, ILogger<GroupUserRoleService> logger)
        {
            _roleRepository = roleRepository;
            _logger = logger;
        }

        // Retrieve group user roles with optional filtering
        public async Task<(List<GroupUserRole> Roles, string Error)> GetGroupUserRolesAsync(Guid requestingUserId, Guid groupId, Guid? groupUserRoleId = null, Guid? memberId = null)
        {
            try
            {
                // Check user's role
                var userRoles = await _roleRepository.GetAsync(groupId: groupId, userId: requestingUserId);
                var userRole = userRoles.FirstOrDefault();
                if (userRole == null) return (new List<GroupUserRole>(), "You are not member of this group.");

                // Check if he has enough rights
                if (userRole.Role != GroupRole.Manager && userRole.Role != GroupRole.Admin)
                {
                    return (new List<GroupUserRole>(), "You don't have enough rights.");
                }

                var roles = await _roleRepository.GetAsync(groupUserRoleId, groupId, memberId);
                return (roles, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving group user roles.");
                return (new List<GroupUserRole>(), "An error occurred while retrieving group user roles.");
            }
        }

        // Create a new group user role
        public async Task<(GroupUserRole? Role, string Error)> CreateGroupUserRoleAsync(Guid groupId, Guid requestingUserId)
        {
            try
            {
                // Check if the user is already a member of the group
                var existingRoles = await _roleRepository.GetAsync(groupId: groupId, userId: requestingUserId);
                if (existingRoles.Any()) return (null, "You are already a member of the group.");

                // Assign default role 'Member' when a user joins a group
                var (groupUserRole, createError) = GroupUserRole.Create(requestingUserId, groupId, GroupRole.Member);
                if (groupUserRole == null) return (null, createError);

                // Save to the database
                var createdRole = await _roleRepository.CreateAsync(groupUserRole);
                return (createdRole, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while joining the group.");
                return (null, "An error occurred while joining the group.");
            }
        }

        // Update an existing group user role
        public async Task<(GroupUserRole? Role, string Error)> UpdateGroupUserRoleAsync(Guid groupUserRoleId, Guid requestingUserId, GroupRole newRole)
        {
            try
            {
                // Retrieve the role to be updated
                var roles = await _roleRepository.GetAsync(groupUserRoleId: groupUserRoleId);
                var groupUserRole = roles.FirstOrDefault();
                if (groupUserRole == null) return (null, "Group user role not found.");

                // Retrieve the requesting user's role in the same group
                var requesterRoles = await _roleRepository.GetAsync(groupId: groupUserRole.GroupId, userId: requestingUserId);
                var requesterRole = requesterRoles.FirstOrDefault();

                if (requesterRole == null || requesterRole.Role != GroupRole.Admin)
                {
                    return (null, "Only Admins can update user roles.");
                }

                // Prevent self-demotion or unauthorized changes
                if (groupUserRole.UserId == requestingUserId && newRole != GroupRole.Admin)
                {
                    return (null, "Admins cannot change their own role to a lower level.");
                }

                // Create updated role model
                var (updatedUserRole, createError) = GroupUserRole.CreateFromDb(
                    groupUserRole.Id,
                    groupUserRole.UserId,
                    groupUserRole.GroupId,
                    newRole,
                    groupUserRole.CreatedAt,
                    DateTime.UtcNow);

                if (updatedUserRole == null) return (null, createError);

                // Update in the database
                var result = await _roleRepository.UpdateAsync(groupUserRoleId, updatedUserRole);
                return (result, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the group user role.");
                return (null, "An error occurred while updating the group user role.");
            }
        }

        // Delete a group user role
        public async Task<(Guid Id, string Error)> DeleteGroupUserRoleAsync(Guid groupUserRoleId, Guid requestingUserId)
        {
            try
            {
                // Retrieve the role to be deleted
                var roles = await _roleRepository.GetAsync(groupUserRoleId: groupUserRoleId);
                var groupUserRole = roles.FirstOrDefault();
                if (groupUserRole == null) return (Guid.Empty, "Group user role not found.");

                // Retrieve the requesting user's role in the same group
                var requesterRoles = await _roleRepository.GetAsync(groupId: groupUserRole.GroupId, userId: requestingUserId);
                var requesterRole = requesterRoles.FirstOrDefault();

                // Allow the user to delete their own role (leave the group)
                if (groupUserRole.UserId == requestingUserId)
                {
                    // Proceed to delete the user's own role
                    var deletedId = await _roleRepository.DeleteAsync(groupUserRoleId);
                    return (deletedId, string.Empty);
                }

                // If the requester is not the user themselves, check if they are a Manager or Admin
                if (requesterRole == null || (requesterRole.Role != GroupRole.Manager && requesterRole.Role != GroupRole.Admin))
                {
                    return (Guid.Empty, "Only Managers and Admins can delete other users' group roles.");
                }

                // Delete from the database
                var deletedIdAuthorized = await _roleRepository.DeleteAsync(groupUserRoleId);
                return (deletedIdAuthorized, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the group user role.");
                return (Guid.Empty, "An error occurred while deleting the group user role.");
            }
        }
    }
}

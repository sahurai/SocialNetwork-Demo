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
        public async Task<(List<GroupUserRole> Roles, string Error)> GetGroupUserRolesAsync(Guid? groupUserRoleId = null, Guid? groupId = null, Guid? userId = null)
        {
            try
            {
                var roles = await _roleRepository.GetAsync(groupUserRoleId, groupId, userId);
                return (roles, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving group user roles.");
                return (new List<GroupUserRole>(), "An error occurred while retrieving group user roles.");
            }
        }

        // Create a new group user role
        public async Task<(GroupUserRole? Role, string Error)> CreateGroupUserRoleAsync(Guid groupId, Guid requestingUserId, GroupRole role)
        {
            try
            {
                // Create the role model
                var (groupUserRole, createError) = GroupUserRole.Create(requestingUserId, groupId, role);
                if (groupUserRole == null) return (null, createError);

                // Save to the database
                var createdRole = await _roleRepository.CreateAsync(groupUserRole);
                return (createdRole, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a group user role.");
                return (null, "An error occurred while creating the group user role.");
            }
        }

        // Update an existing group user role
        public async Task<(GroupUserRole? Role, string Error)> UpdateGroupUserRoleAsync(Guid groupUserRoleId, Guid requestingUserId, GroupRole role)
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
                if (requesterRole == null)
                {
                    return (null, "Requesting user does not have a role in this group.");
                }

                // If updating the role to Manager, ensure the requester is an Admin
                if (role == GroupRole.Manager && requesterRole.Role != GroupRole.Admin)
                {
                    return (null, "Only Admins can assign the Manager role.");
                }

                // Revert to using model creation during update
                var (updatedUserRole, createError) = GroupUserRole.CreateFromDb(
                    groupUserRole.Id,
                    groupUserRole.UserId,
                    groupUserRole.GroupId,
                    role,
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

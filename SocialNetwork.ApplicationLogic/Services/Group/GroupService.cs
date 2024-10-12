using Microsoft.Extensions.Logging;
using SocialNetwork.Core.Enums;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Repositories;

namespace SocialNetwork.ApplicationLogic.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupUserRoleRepository _groupUserRoleRepository;
        private readonly ILogger<GroupService> _logger;

        public GroupService(
            IGroupRepository groupRepository,
            IGroupUserRoleRepository groupUserRoleRepository,
            ILogger<GroupService> logger)
        {
            _groupRepository = groupRepository;
            _groupUserRoleRepository = groupUserRoleRepository;
            _logger = logger;
        }

        // Retrieve groups with optional filtering
        public async Task<(List<Group> Groups, string Error)> GetGroupsAsync(
            Guid? groupId = null,
            Guid? creatorId = null,
            string? name = null,
            string? description = null)
        {
            try
            {
                var groups = await _groupRepository.GetAsync(groupId, creatorId, name, description);
                return (groups, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving groups.");
                return (new List<Group>(), "An error occurred while retrieving groups.");
            }
        }

        // Create a new group
        public async Task<(Group? Group, string Error)> CreateGroupAsync(Guid requestingUserId, string name, string? description)
        {
            try
            {
                // Create the group model
                var (group, createError) = Group.Create(requestingUserId, name, description);
                if (group == null) return (null, createError);

                // Save to the database
                var createdGroup = await _groupRepository.CreateAsync(group);
                if (createdGroup == null) return (null, "Failed to create the group.");

                // Assign the Admin role to the requesting user
                var (groupUserRole, roleError) = GroupUserRole.Create(
                    groupId: createdGroup.Id,
                    userId: requestingUserId,
                    role: GroupRole.Admin
                );

                if (groupUserRole == null)
                {
                    // If role assignment fails, delete the created group to maintain data integrity
                    await _groupRepository.DeleteAsync(createdGroup.Id);
                    return (null, roleError);
                }

                // Save the user role to the database
                var createdRole = await _groupUserRoleRepository.CreateAsync(groupUserRole);
                if (createdRole == null)
                {
                    // If role assignment fails, delete the created group to maintain data integrity
                    await _groupRepository.DeleteAsync(createdGroup.Id);
                    return (null, "Failed to assign Admin role to the user.");
                }

                return (createdGroup, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a group.");
                return (null, "An error occurred while creating the group.");
            }
        }

        // Update an existing group
        public async Task<(Group? Group, string Error)> UpdateGroupAsync(
            Guid groupId,
            Guid requestingUserId,
            string? name,
            string? description)
        {
            try
            {
                // Retrieve the group
                var groups = await _groupRepository.GetAsync(groupId: groupId);
                var group = groups.FirstOrDefault();
                if (group == null) return (null, "Group not found.");

                // Check if the user has at least Admin role in the group
                var userRoles = await _groupUserRoleRepository.GetAsync(groupId: groupId, userId: requestingUserId);
                if (!userRoles.Any(gur => gur.Role == GroupRole.Admin))
                    return (null, "You must be an Admin to update this group.");

                // Update fields
                string updatedName = name ?? group.Name;
                string? updatedDescription = description ?? group.Description;

                // Create updated group model
                var (updatedGroup, createError) = Group.CreateFromDb(
                    group.Id,
                    group.CreatorId,
                    updatedName,
                    updatedDescription,
                    group.CreatedAt,
                    DateTime.UtcNow);

                if (updatedGroup == null) return (null, createError);

                // Update in the database
                var result = await _groupRepository.UpdateAsync(groupId, updatedGroup);
                return (result, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the group.");
                return (null, "An error occurred while updating the group.");
            }
        }

        // Delete a group
        public async Task<(Guid Id, string Error)> DeleteGroupAsync(Guid groupId, Guid requestingUserId)
        {
            try
            {
                // Retrieve the group
                var groups = await _groupRepository.GetAsync(groupId: groupId);
                var group = groups.FirstOrDefault();
                if (group == null) return (Guid.Empty, "Group not found.");

                // Check if the user has at least Admin role in the group
                var userRoles = await _groupUserRoleRepository.GetAsync(groupId: groupId, userId: requestingUserId);
                if (!userRoles.Any(gur => gur.Role == GroupRole.Admin))
                    return (Guid.Empty, "You must be an Admin to delete this group.");

                // Delete from the database
                var deletedId = await _groupRepository.DeleteAsync(groupId);
                return (deletedId, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the group.");
                return (Guid.Empty, "An error occurred while deleting the group.");
            }
        }
    }
}

using Microsoft.Extensions.Logging;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Repositories;

namespace SocialNetwork.ApplicationLogic.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ILogger<GroupService> _logger;

        public GroupService(IGroupRepository groupRepository, ILogger<GroupService> logger)
        {
            _groupRepository = groupRepository;
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
            Guid? creatorId,
            string? name,
            string? description)
        {
            try
            {
                // Retrieve the group
                var groups = await _groupRepository.GetAsync(groupId: groupId);
                var group = groups.FirstOrDefault();
                if (group == null) return (null, "Group not found.");

                // Ensure the requester is the creator of the group
                if (group.CreatorId != requestingUserId)
                    return (null, "You can only update your own groups.");

                // Update fields
                Guid updatedCreatorId = creatorId ?? group.CreatorId;
                string updatedName = name ?? group.Name;
                string? updatedDescription = description ?? group.Description;

                // Create updated group model
                var (updatedGroup, createError) = Group.CreateFromDb(
                    group.Id,
                    updatedCreatorId,
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

                // Check if the requesting user is the creator
                if (group.CreatorId != requestingUserId)
                    return (Guid.Empty, "You can only delete your own groups.");

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

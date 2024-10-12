    using Microsoft.Extensions.Logging;
    using SocialNetwork.Core.Enums;
    using SocialNetwork.Core.Models;
    using SocialNetwork.DataAccess.Repositories;

    namespace SocialNetwork.ApplicationLogic.Services
    {
        public class GroupBlockService : IGroupBlockService
        {
            private readonly IGroupBlockRepository _blockRepository;
            private readonly IGroupUserRoleRepository _groupUserRoleRepository;
            private readonly ILogger<GroupBlockService> _logger;

            public GroupBlockService(
                IGroupBlockRepository blockRepository,
                IGroupUserRoleRepository groupUserRoleRepository,
                ILogger<GroupBlockService> logger)
            {
                _blockRepository = blockRepository;
                _groupUserRoleRepository = groupUserRoleRepository;
                _logger = logger;
            }

            // Retrieve group blocks with optional filtering
            public async Task<(List<GroupBlock> Blocks, string Error)> GetGroupBlocksAsync(
                Guid requestingUserId,
                Guid groupId,
                Guid? blockId = null,
                Guid? blockerId = null,
                Guid? blockedId = null)
            {
                try
                {
                    // Check user's role
                    var userRoles = await _groupUserRoleRepository.GetAsync(groupId: groupId, userId: requestingUserId);
                    var userRole = userRoles.FirstOrDefault();
                    if (userRole == null) return (new List<GroupBlock>(), "You are not member of this group.");

                    // Check if he has enough rights
                    if (userRole.Role != GroupRole.Manager && userRole.Role != GroupRole.Admin)
                    {
                        return (new List<GroupBlock>(), "You don't have enough rights.");
                    }

                    // Get data
                    var blocks = await _blockRepository.GetAsync(blockId, groupId, blockerId, blockedId);

                    return (blocks, string.Empty);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while retrieving group blocks.");
                    return (new List<GroupBlock>(), "An error occurred while retrieving group blocks.");
                }
            }

            // Create a new group block
            public async Task<(GroupBlock? Block, string Error)> CreateGroupBlockAsync(Guid groupId, Guid requestingUserId, Guid blockedId)
                {
                    try
                    {
                        // Check if the blocker is a manager or admin in the group
                        var roles = await _groupUserRoleRepository.GetAsync(groupId: groupId, userId: requestingUserId);
                        var blockerRole = roles.FirstOrDefault();
                        if (blockerRole == null || (blockerRole.Role != GroupRole.Manager && blockerRole.Role != GroupRole.Admin))
                        {
                            return (null, "Only managers, admins or creator can block users in the group.");
                        }

                        // Create the block model
                        var (groupBlock, createError) = GroupBlock.Create(requestingUserId, blockedId, groupId);
                        if (groupBlock == null) return (null, createError);

                        // Save to the database
                        var createdBlock = await _blockRepository.CreateAsync(groupBlock);
                        return (createdBlock, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while creating a group block.");
                        return (null, "An error occurred while creating the group block.");
                    }
                }

            // Delete a group block
            public async Task<(Guid Id, string Error)> DeleteGroupBlockAsync(Guid id, Guid requestingUserId)
            {
                try
                {
                    // Retrieve the block
                    var blocks = await _blockRepository.GetAsync(groupBlockId: id);
                    var groupBlock = blocks.FirstOrDefault();
                    if (groupBlock == null) return (Guid.Empty, "Group block not found.");

                    // Check if the requesting user is a manager or admin in the group
                    var roles = await _groupUserRoleRepository.GetAsync(groupId: groupBlock.GroupId, userId: requestingUserId);
                    var userRole = roles.FirstOrDefault();
                    if (userRole == null || (userRole.Role != GroupRole.Manager && userRole.Role != GroupRole.Admin))
                    {
                        return (Guid.Empty, "Only managers, admins or creator can delete group blocks.");
                    }

                    // Delete from the database
                    var deletedId = await _blockRepository.DeleteAsync(id);
                    return (deletedId, string.Empty);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while deleting the group block.");
                    return (Guid.Empty, "An error occurred while deleting the group block.");
                }
            }
        }
    }

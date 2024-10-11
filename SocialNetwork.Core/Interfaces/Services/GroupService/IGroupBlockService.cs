using SocialNetwork.Core.Models;

namespace SocialNetwork.ApplicationLogic.Services
{
    public interface IGroupBlockService
    {
        Task<(GroupBlock? Block, string Error)> CreateGroupBlockAsync(Guid groupId, Guid requestingUserId, Guid blockedId);
        Task<(Guid Id, string Error)> DeleteGroupBlockAsync(Guid id, Guid requestingUserId);
        Task<(List<GroupBlock> Blocks, string Error)> GetGroupBlocksAsync(Guid? groupBlockId = null, Guid? groupId = null, Guid? blockerId = null, Guid? blockedId = null);
    }
}
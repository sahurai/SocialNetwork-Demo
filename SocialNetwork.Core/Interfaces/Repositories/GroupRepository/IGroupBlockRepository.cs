using SocialNetwork.Core.Models;

namespace SocialNetwork.DataAccess.Repositories
{
    public interface IGroupBlockRepository
    {
        Task<GroupBlock> CreateAsync(GroupBlock groupBlock);
        Task<Guid> DeleteAsync(Guid id);
        Task<List<GroupBlock>> GetAsync(Guid? groupBlockId = null, Guid? groupId = null, Guid? blockerId = null, Guid? blockedId = null);
    }
}
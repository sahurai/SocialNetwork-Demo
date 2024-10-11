using SocialNetwork.Core.Models;

namespace SocialNetwork.DataAccess.Repositories
{
    public interface IUserBlockRepository
    {
        Task<UserBlock> CreateAsync(UserBlock userBlock);
        Task<Guid> DeleteAsync(Guid id);
        Task<List<UserBlock>> GetAsync(Guid? userBlockId = null, Guid? blockerId = null, Guid? blockedId = null);
    }
}
using SocialNetwork.Core.Models;

namespace SocialNetwork.DataAccess.Repositories
{
    public interface IGroupRepository
    {
        Task<Group> CreateAsync(Group group);
        Task<Guid> DeleteAsync(Guid id);
        Task<List<Group>> GetAsync(Guid? groupId = null, Guid? creatorId = null, string? name = null, string? description = null);
        Task<Group> UpdateAsync(Guid id, Group updatedGroup);
    }
}
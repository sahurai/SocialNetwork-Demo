using SocialNetwork.Core.Models;

namespace SocialNetwork.DataAccess.Repositories
{
    public interface IGroupUserRoleRepository
    {
        Task<GroupUserRole> CreateAsync(GroupUserRole groupUserRole);
        Task<Guid> DeleteAsync(Guid id);
        Task<List<GroupUserRole>> GetAsync(Guid? groupUserRoleId = null, Guid? groupId = null, Guid? userId = null);
        Task<GroupUserRole> UpdateAsync(Guid id, GroupUserRole updatedGroupUserRole);
    }
}
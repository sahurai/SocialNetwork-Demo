using SocialNetwork.Core.Enums;
using SocialNetwork.Core.Models;

namespace SocialNetwork.ApplicationLogic.Services
{
    public interface IGroupUserRoleService
    {
        Task<(GroupUserRole? Role, string Error)> CreateGroupUserRoleAsync(Guid groupId, Guid requestingUserId, GroupRole role);
        Task<(Guid Id, string Error)> DeleteGroupUserRoleAsync(Guid groupUserRoleId, Guid requestingUserId);
        Task<(List<GroupUserRole> Roles, string Error)> GetGroupUserRolesAsync(Guid? groupUserRoleId = null, Guid? groupId = null, Guid? userId = null);
        Task<(GroupUserRole? Role, string Error)> UpdateGroupUserRoleAsync(Guid groupUserRoleId, Guid requestingUserId, GroupRole role);
    }
}
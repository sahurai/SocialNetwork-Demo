using SocialNetwork.Core.Enums;
using SocialNetwork.Core.Models;

namespace SocialNetwork.ApplicationLogic.Services
{
    public interface IGroupUserRoleService
    {
        Task<(GroupUserRole? Role, string Error)> CreateGroupUserRoleAsync(Guid groupId, Guid requestingUserId);
        Task<(Guid Id, string Error)> DeleteGroupUserRoleAsync(Guid groupUserRoleId, Guid requestingUserId);
        Task<(List<GroupUserRole> Roles, string Error)> GetGroupUserRolesAsync(Guid requestingUserId, Guid groupId, Guid? groupUserRoleId = null, Guid? memberId = null);
        Task<(GroupUserRole? Role, string Error)> UpdateGroupUserRoleAsync(Guid groupUserRoleId, Guid requestingUserId, GroupRole newRole);
    }
}
using SocialNetwork.Core.Models;

namespace SocialNetwork.ApplicationLogic.Services
{
    public interface IGroupService
    {
        Task<(Group? Group, string Error)> CreateGroupAsync(Guid requestingUserId, string name, string? description);
        Task<(Guid Id, string Error)> DeleteGroupAsync(Guid groupId, Guid requestingUserId);
        Task<(List<Group> Groups, string Error)> GetGroupsAsync(Guid? groupId = null, Guid? creatorId = null, string? name = null, string? description = null);
        Task<(Group? Group, string Error)> UpdateGroupAsync(Guid groupId, Guid requestingUserId, string? name, string? description);
    }
}
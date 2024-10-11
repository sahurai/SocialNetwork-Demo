using SocialNetwork.Core.Models;

namespace SocialNetwork.ApplicationLogic.Services
{
    public interface IUserBlockService
    {
        Task<(UserBlock? Block, string Error)> CreateUserBlockAsync(Guid requestingUserId, Guid blockedId);
        Task<(Guid Id, string Error)> DeleteUserBlockAsync(Guid userBlockId, Guid requestingUserId);
        Task<(List<UserBlock> Blocks, string Error)> GetUserBlocksAsync(Guid? blockerId = null, Guid? blockedId = null);
    }
}
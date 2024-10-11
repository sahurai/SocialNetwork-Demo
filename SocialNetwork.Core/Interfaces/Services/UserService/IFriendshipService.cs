using SocialNetwork.Core.Models;

namespace SocialNetwork.ApplicationLogic.Services
{
    public interface IFriendshipService
    {
        Task<(Friendship? Friendship, string Error)> AcceptFriendshipAsync(Guid friendshipId, Guid userId);
        Task<(Friendship? Friendship, string Error)> CreateFriendshipAsync(Guid requestingUserId, Guid user2Id);
        Task<(Guid Id, string Error)> DeleteFriendshipAsync(Guid friendshipId, Guid requestingUserId);
        Task<(List<Friendship> Friendships, string Error)> GetFriendshipsAsync(Guid userId);
    }
}
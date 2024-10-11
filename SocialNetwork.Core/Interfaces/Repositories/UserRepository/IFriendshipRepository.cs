using SocialNetwork.Core.Models;

namespace SocialNetwork.DataAccess.Repositories
{
    public interface IFriendshipRepository
    {
        Task<Friendship> CreateAsync(Friendship friendship);
        Task<Guid> DeleteAsync(Guid id);
        Task<List<Friendship>> GetAsync(Guid? friendshipId = null, Guid? userId = null);
        Task<Friendship?> GetFriendshipBetweenUsersAsync(Guid userId1, Guid userId2);
        Task<Friendship> UpdateAsync(Guid id, Friendship updatedFriendship);
    }
}
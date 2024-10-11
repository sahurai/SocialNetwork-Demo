using SocialNetwork.Core.Models;

namespace SocialNetwork.DataAccess.Repositories
{
    public interface ILikeRepository
    {
        Task<Like> CreateAsync(Like like);
        Task<Guid> DeleteAsync(Guid id);
        Task<List<Like>> GetAsync(Guid? likeId = null, Guid? userId = null, Guid? postId = null, Guid? commentId = null);
    }
}
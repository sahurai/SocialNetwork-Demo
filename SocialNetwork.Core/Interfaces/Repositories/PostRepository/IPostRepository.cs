using SocialNetwork.Core.Models;

namespace SocialNetwork.Core.Interfaces.Repositories.PostRepository
{
    public interface IPostRepository
    {
        Task<Post> CreateAsync(Post post);
        Task<Guid> DeleteAsync(Guid id);
        Task<List<Post>> GetAsync(Guid? postId = null, Guid? authorId = null, Guid? groupId = null, string? content = null);
        Task<Post> UpdateAsync(Guid id, Post updatedPost);
    }
}
using SocialNetwork.Core.Models;

namespace SocialNetwork.DataAccess.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment> CreateAsync(Comment comment);
        Task<Guid> DeleteAsync(Guid id);
        Task<List<Comment>> GetAsync(Guid? commentId = null, Guid? postId = null, Guid? authorId = null, string? content = null);
        Task<Comment> UpdateAsync(Guid id, Comment updatedComment);
    }
}
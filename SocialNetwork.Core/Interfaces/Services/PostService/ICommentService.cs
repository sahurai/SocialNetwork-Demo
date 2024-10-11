using SocialNetwork.Core.Models;

namespace SocialNetwork.ApplicationLogic.Services
{
    public interface ICommentService
    {
        Task<(Comment? Comment, string Error)> CreateCommentAsync(Guid requestingUserId, Guid postId, string content);
        Task<(Guid Id, string Error)> DeleteCommentAsync(Guid commentId, Guid requestingUserId);
        Task<(List<Comment> Comments, string Error)> GetCommentsAsync(Guid? commentId = null, Guid? postId = null, Guid? authorId = null, string? content = null);
        Task<(Comment? Comment, string Error)> UpdateCommentAsync(Guid commentId, Guid requestingUserId, string content);
    }
}
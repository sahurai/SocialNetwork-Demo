using SocialNetwork.Core.Models;

namespace SocialNetwork.ApplicationLogic.Services
{
    public interface IPostService
    {
        Task<(Post? Post, string Error)> CreatePostAsync(Guid requestingUserId, string content, Guid? groupId = null);
        Task<(Guid Id, string Error)> DeletePostAsync(Guid postId, Guid requestingUserId);
        Task<(List<Post> Posts, string Error)> GetPostsAsync(Guid? postId = null, Guid? authorId = null, Guid? groupId = null, string? content = null);
        Task<(Post? Post, string Error)> UpdatePostAsync(Guid postId, Guid requestingUserId, string content);
    }
}
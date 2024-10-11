using SocialNetwork.Core.Models;

namespace SocialNetwork.ApplicationLogic.Services
{
    public interface ILikeService
    {
        Task<(Like? Like, string Error)> CreateLikeAsync(Guid userId, Guid? postId = null, Guid? commentId = null);
        Task<(Guid Id, string Error)> DeleteLikeAsync(Guid id, Guid requestingUserId);
        Task<(List<Like> Likes, string Error)> GetLikesAsync(Guid? likeId = null, Guid? userId = null, Guid? postId = null, Guid? commentId = null);
    }
}
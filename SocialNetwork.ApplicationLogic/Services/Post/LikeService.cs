using Microsoft.Extensions.Logging;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Repositories;

namespace SocialNetwork.ApplicationLogic.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly ILogger<LikeService> _logger;

        public LikeService(ILikeRepository likeRepository, ILogger<LikeService> logger)
        {
            _likeRepository = likeRepository;
            _logger = logger;
        }

        // Retrieve likes with optional filtering
        public async Task<(List<Like> Likes, string Error)> GetLikesAsync(Guid? likeId = null, Guid? userId = null, Guid? postId = null, Guid? commentId = null)
        {
            try
            {
                var likes = await _likeRepository.GetAsync(likeId, userId, postId, commentId);
                return (likes, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving likes.");
                return (new List<Like>(), "An error occurred while retrieving likes.");
            }
        }

        // Create a new like
        public async Task<(Like? Like, string Error)> CreateLikeAsync(Guid userId, Guid? postId = null, Guid? commentId = null)
        {
            try
            {
                // Create the like model
                var (like, createError) = Like.Create(userId, postId, commentId);
                if (like == null) return (null, createError);

                // Save to the database
                var createdLike = await _likeRepository.CreateAsync(like);
                return (createdLike, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a like.");
                return (null, "An error occurred while creating the like.");
            }
        }

        // Delete a like
        public async Task<(Guid Id, string Error)> DeleteLikeAsync(Guid id, Guid requestingUserId)
        {
            try
            {
                // Retrieve the like
                var likes = await _likeRepository.GetAsync(likeId: id);
                var like = likes.FirstOrDefault();
                if (like == null) return (Guid.Empty, "Like not found.");

                // Check if the requesting user is the one who liked
                if (like.UserId != requestingUserId) return (Guid.Empty, "You can only delete your own likes.");

                // Delete from the database
                var deletedId = await _likeRepository.DeleteAsync(id);
                return (deletedId, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the like.");
                return (Guid.Empty, "An error occurred while deleting the like.");
            }
        }
    }
}

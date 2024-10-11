using Microsoft.EntityFrameworkCore;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Entities.Post;

namespace SocialNetwork.DataAccess.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly SocialNetworkDbContext _context;

        public LikeRepository(SocialNetworkDbContext context)
        {
            _context = context;
        }

        // Retrieve likes with optional filtering
        public async Task<List<Like>> GetAsync(Guid? likeId = null, Guid? userId = null, Guid? postId = null, Guid? commentId = null)
        {
            IQueryable<LikeEntity> query = _context.Likes.AsNoTracking();

            if (likeId.HasValue)
            {
                query = query.Where(like => like.Id == likeId.Value);
            }

            if (userId.HasValue)
            {
                query = query.Where(like => like.UserId == userId.Value);
            }

            if (postId.HasValue)
            {
                query = query.Where(like => like.PostId == postId.Value);
            }

            if (commentId.HasValue)
            {
                query = query.Where(like => like.CommentId == commentId.Value);
            }

            query = query.OrderByDescending(like => like.CreatedAt);

            List<LikeEntity> likeEntities = await query.ToListAsync();

            List<Like> likes = likeEntities.Select(MapToModel).ToList();

            return likes;
        }

        // Create a new like
        public async Task<Like> CreateAsync(Like like)
        {
            LikeEntity likeEntity = new LikeEntity
            {
                UserId = like.UserId,
                PostId = like.PostId,
                CommentId = like.CommentId
            };

            await _context.Likes.AddAsync(likeEntity);
            await _context.SaveChangesAsync();

            return MapToModel(likeEntity);
        }

        // Delete a like by Id
        public async Task<Guid> DeleteAsync(Guid id)
        {
            await _context.Likes
                .Where(like => like.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }

        // Map LikeEntity to Like model
        private Like MapToModel(LikeEntity entity)
        {
            var (like, error) = Like.CreateFromDb(
                entity.Id,
                entity.UserId,
                entity.PostId,
                entity.CommentId,
                entity.CreatedAt,
                entity.UpdatedAt);

            if (!string.IsNullOrEmpty(error)) throw new Exception($"Error during mapping: {error}");

            return like!;
        }
    }
}

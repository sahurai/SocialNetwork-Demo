using Microsoft.EntityFrameworkCore;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Entities.Post;


namespace SocialNetwork.DataAccess.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly SocialNetworkDbContext _context;

        public CommentRepository(SocialNetworkDbContext context)
        {
            _context = context;
        }

        // Retrieve comments with optional filtering
        public async Task<List<Comment>> GetAsync(
            Guid? commentId = null,
            Guid? postId = null,
            Guid? authorId = null,
            string? content = null)
        {
            IQueryable<CommentEntity> query = _context.Comments.AsNoTracking();

            if (commentId.HasValue)
            {
                query = query.Where(comment => comment.Id == commentId.Value);
            }

            if (postId.HasValue)
            {
                query = query.Where(comment => comment.PostId == postId.Value);
            }

            if (authorId.HasValue)
            {
                query = query.Where(comment => comment.AuthorId == authorId.Value);
            }

            if (!string.IsNullOrEmpty(content))
            {
                query = query.Where(comment => comment.Content.ToLower().Contains(content.ToLower()));
            }

            query = query.OrderByDescending(comment => comment.CreatedAt);

            List<CommentEntity> commentEntities = await query.ToListAsync();

            List<Comment> comments = commentEntities.Select(MapToModel).ToList();

            return comments;
        }

        // Create a new comment
        public async Task<Comment> CreateAsync(Comment comment)
        {
            CommentEntity commentEntity = new CommentEntity
            {
                AuthorId = comment.AuthorId,
                PostId = comment.PostId,
                Content = comment.Content
            };

            await _context.Comments.AddAsync(commentEntity);
            await _context.SaveChangesAsync();

            return MapToModel(commentEntity);
        }

        // Update an existing comment
        public async Task<Comment> UpdateAsync(Guid id, Comment updatedComment)
        {
            await _context.Comments
                .Where(comment => comment.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(comment => comment.Content, updatedComment.Content)
                    .SetProperty(comment => comment.EditedAt, updatedComment.EditedAt)
                    .SetProperty(comment => comment.UpdatedAt, updatedComment.UpdatedAt));

            return updatedComment;
        }

        // Delete a comment by Id
        public async Task<Guid> DeleteAsync(Guid id)
        {
            await _context.Comments
                .Where(comment => comment.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }

        // Map CommentEntity to Comment model
        private Comment MapToModel(CommentEntity entity)
        {
            var (comment, error) = Comment.CreateFromDb(
                entity.Id,
                entity.AuthorId,
                entity.PostId,
                entity.Content,
                entity.EditedAt,
                entity.CreatedAt,
                entity.UpdatedAt);

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception($"Error during mapping: {error}");
            }

            return comment!;
        }
    }
}

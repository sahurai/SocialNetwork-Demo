using Microsoft.EntityFrameworkCore;
using SocialNetwork.Core.Interfaces.Repositories.PostRepository;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Entities.Post;


namespace SocialNetwork.DataAccess.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly SocialNetworkDbContext _context;

        public PostRepository(SocialNetworkDbContext context)
        {
            _context = context;
        }

        // Retrieve posts with optional filtering
        public async Task<List<Post>> GetAsync(Guid? postId = null, Guid? authorId = null, Guid? groupId = null, string? content = null)
        {
            IQueryable<PostEntity> query = _context.Posts.AsNoTracking();

            if (postId.HasValue)
            {
                query = query.Where(post => post.Id == postId.Value);
            }

            if (authorId.HasValue)
            {
                query = query.Where(post => post.AuthorId == authorId.Value);
            }

            if (groupId.HasValue)
            {
                query = query.Where(post => post.GroupId == groupId.Value);
            }

            if (!string.IsNullOrEmpty(content))
            {
                query = query.Where(post => post.Content.ToLower().Contains(content.ToLower()));
            }

            query = query.OrderByDescending(post => post.CreatedAt);

            List<PostEntity> postEntities = await query.ToListAsync();

            List<Post> posts = postEntities.Select(MapToModel).ToList();

            return posts;
        }

        // Create a new post
        public async Task<Post> CreateAsync(Post post)
        {
            PostEntity postEntity = new PostEntity
            {
                AuthorId = post.AuthorId,
                Content = post.Content,
                GroupId = post.GroupId
            };

            await _context.Posts.AddAsync(postEntity);
            await _context.SaveChangesAsync();

            return MapToModel(postEntity);
        }

        // Update an existing post
        public async Task<Post> UpdateAsync(Guid id, Post updatedPost)
        {
            await _context.Posts
               .Where(post => post.Id == id)
               .ExecuteUpdateAsync(s => s
                   .SetProperty(post => post.Content, updatedPost.Content)
                   .SetProperty(post => post.EditedAt, updatedPost.EditedAt)
                   .SetProperty(post => post.UpdatedAt, updatedPost.UpdatedAt));

            return updatedPost;
        }

        // Delete a post by Id
        public async Task<Guid> DeleteAsync(Guid id)
        {
            await _context.Posts
                .Where(post => post.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }

        // Map PostEntity to Post model
        private Post MapToModel(PostEntity entity)
        {
            var (post, error) = Post.CreateFromDb(
                entity.Id,
                entity.AuthorId,
                entity.Content,
                entity.GroupId,
                entity.EditedAt,
                entity.CreatedAt,
                entity.UpdatedAt);

            if (!string.IsNullOrEmpty(error)) throw new Exception($"Error during mapping: {error}");

            return post!;
        }
    }
}

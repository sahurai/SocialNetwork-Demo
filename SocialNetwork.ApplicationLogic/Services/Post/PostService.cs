using Microsoft.Extensions.Logging;
using SocialNetwork.Core.Enums;
using SocialNetwork.Core.Interfaces.Repositories.PostRepository;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Repositories;

namespace SocialNetwork.ApplicationLogic.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IGroupUserRoleRepository _groupUserRoleRepository;
        private readonly ILogger<PostService> _logger;

        public PostService(
            IPostRepository postRepository,
            IGroupUserRoleRepository groupUserRoleRepository,
            ILogger<PostService> logger)
        {
            _postRepository = postRepository;
            _groupUserRoleRepository = groupUserRoleRepository;
            _logger = logger;
        }

        // Retrieve posts with optional filtering
        public async Task<(List<Post> Posts, string Error)> GetPostsAsync(
            Guid? postId = null,
            Guid? authorId = null,
            Guid? groupId = null,
            string? content = null)
        {
            try
            {
                var posts = await _postRepository.GetAsync(postId, authorId, groupId, content);
                return (posts, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving posts.");
                return (new List<Post>(), "An error occurred while retrieving posts.");
            }
        }

        // Create a new post
        public async Task<(Post? Post, string Error)> CreatePostAsync(
            Guid requestingUserId,
            string content,
            Guid? groupId = null)
        {
            try
            {
                // If groupId is provided, check if author is Admin or Manager in the group
                if (groupId.HasValue)
                {
                    var roles = await _groupUserRoleRepository.GetAsync(groupId: groupId.Value, userId: requestingUserId);
                    var userRole = roles.FirstOrDefault();
                    if (userRole == null || (userRole.Role != GroupRole.Admin && userRole.Role != GroupRole.Manager))
                    {
                        return (null, "Only group admins or managers can create posts in this group.");
                    }
                }

                // Create the post model
                var (post, createError) = Post.Create(requestingUserId, content, groupId);
                if (post == null) return (null, createError);

                // Save to the database
                var createdPost = await _postRepository.CreateAsync(post);
                return (createdPost, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a post.");
                return (null, "An error occurred while creating the post.");
            }
        }

        // Update an existing post
        public async Task<(Post? Post, string Error)> UpdatePostAsync(
            Guid postId,
            Guid requestingUserId,
            string content)
        {
            try
            {
                // Retrieve the post
                var posts = await _postRepository.GetAsync(postId: postId);
                var post = posts.FirstOrDefault();
                if (post == null) return (null, "Post not found.");

                // Check if post is in a group
                if (post.GroupId.HasValue)
                {
                    // Retrieve the user's role in the group
                    var roles = await _groupUserRoleRepository.GetAsync(groupId: post.GroupId.Value, userId: requestingUserId);
                    var userRole = roles.FirstOrDefault();
                    if (userRole == null || (userRole.Role != GroupRole.Admin && userRole.Role != GroupRole.Manager))
                    {
                        return (null, "Only group admins or managers can update posts in this group.");
                    }
                }
                else
                {
                    // Ensure the author is the same
                    if (post.AuthorId != requestingUserId) return (null, "You can only update your own posts.");
                }

                // Edit the content
                var editError = post.EditContent(content);
                if (!string.IsNullOrEmpty(editError)) return (null, editError);

                // Update in the database
                var result = await _postRepository.UpdateAsync(postId, post);
                return (result, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the post.");
                return (null, "An error occurred while updating the post.");
            }
        }

        // Delete a post
        public async Task<(Guid Id, string Error)> DeletePostAsync(
            Guid postId,
            Guid requestingUserId)
        {
            try
            {
                // Retrieve the post
                var posts = await _postRepository.GetAsync(postId: postId);
                var post = posts.FirstOrDefault();
                if (post == null) return (Guid.Empty, "Post not found.");

                // Check if post is in a group
                if (post.GroupId.HasValue)
                {
                    // Retrieve the user's role in the group
                    var roles = await _groupUserRoleRepository.GetAsync(groupId: post.GroupId.Value, userId: requestingUserId);
                    var userRole = roles.FirstOrDefault();
                    if (userRole == null || (userRole.Role != GroupRole.Admin && userRole.Role != GroupRole.Manager))
                    {
                        return (Guid.Empty, "Only group admins or managers can delete posts in this group.");
                    }
                }
                else
                {
                    // Check if the requesting user is the author
                    if (post.AuthorId != requestingUserId) return (Guid.Empty, "You can only delete your own posts.");
                }

                // Delete from the database
                var deletedId = await _postRepository.DeleteAsync(postId);
                return (deletedId, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the post.");
                return (Guid.Empty, "An error occurred while deleting the post.");
            }
        }
    }
}

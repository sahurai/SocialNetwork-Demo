using Microsoft.Extensions.Logging;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Repositories;

namespace SocialNetwork.ApplicationLogic.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<CommentService> _logger;

        public CommentService(ICommentRepository commentRepository, ILogger<CommentService> logger)
        {
            _commentRepository = commentRepository;
            _logger = logger;
        }

        // Retrieve comments with optional filtering
        public async Task<(List<Comment> Comments, string Error)> GetCommentsAsync(Guid? commentId = null, Guid? postId = null, Guid? authorId = null, string? content = null)
        {
            try
            {
                var comments = await _commentRepository.GetAsync(commentId, postId, authorId, content);
                return (comments, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving comments.");
                return (new List<Comment>(), "An error occurred while retrieving comments.");
            }
        }

        // Create a new comment
        public async Task<(Comment? Comment, string Error)> CreateCommentAsync(Guid requestingUserId, Guid postId, string content)
        {
            try
            {
                // Create the comment model
                var (comment, createError) = Comment.Create(requestingUserId, postId, content);
                if (comment == null) return (null, createError);

                // Save to the database
                var createdComment = await _commentRepository.CreateAsync(comment);
                return (createdComment, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a comment.");
                return (null, "An error occurred while creating the comment.");
            }
        }

        // Update an existing comment
        public async Task<(Comment? Comment, string Error)> UpdateCommentAsync(Guid commentId, Guid requestingUserId, string content)
        {
            try
            {
                // Retrieve the comment
                var comments = await _commentRepository.GetAsync(commentId: commentId);
                var comment = comments.FirstOrDefault();
                if (comment == null) return (null, "Comment not found.");

                // Ensure the author is the same
                if (comment.AuthorId != requestingUserId) return (null, "You can only update your own comments.");

                // Edit the content
                var editError = comment.EditContent(content);
                if (!string.IsNullOrEmpty(editError)) return (null, editError);

                // Update in the database
                var result = await _commentRepository.UpdateAsync(commentId, comment);
                return (result, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the comment.");
                return (null, "An error occurred while updating the comment.");
            }
        }

        // Delete a comment
        public async Task<(Guid Id, string Error)> DeleteCommentAsync(Guid commentId, Guid requestingUserId)
        {
            try
            {
                // Retrieve the comment
                var comments = await _commentRepository.GetAsync(commentId: commentId);
                var comment = comments.FirstOrDefault();
                if (comment == null) return (Guid.Empty, "Comment not found.");

                // Check if the requesting user is the author
                if (comment.AuthorId != requestingUserId) return (Guid.Empty, "You can only delete your own comments.");

                // Delete from the database
                var deletedId = await _commentRepository.DeleteAsync(commentId);
                return (deletedId, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the comment.");
                return (Guid.Empty, "An error occurred while deleting the comment.");
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using System.Data;
using SocialNetwork.API.DTO.Post;
using Microsoft.AspNetCore.Authorization;
using SocialNetwork.Shared;

namespace SocialNetwork.API.Areas.User.Controllers.Post
{
    [ApiController]
    [Authorize]
    [Area("User")]
    [Route("comments")]
    [ApiExplorerSettings(GroupName = "User")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        // GET: comments
        [HttpGet]
        public async Task<IActionResult> GetComments([FromQuery] Guid? commentId, [FromQuery] Guid? postId, [FromQuery] Guid? authorId, [FromQuery] string? content)
        {
            var (comments, error) = await _commentService.GetCommentsAsync(commentId, postId, authorId, content);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Error = error });

            var response = comments.Select(comment => new CommentResponse
            {
                Id = comment.Id,
                AuthorId = comment.AuthorId,
                PostId = comment.PostId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            }).ToList();

            return Ok(response);
        }

        // POST: comments
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentByUserRequest request)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (comment, error) = await _commentService.CreateCommentAsync(userId.Value, request.PostId, request.Content);
            if (!string.IsNullOrEmpty(error) || comment == null) return BadRequest(new { Error = error });

            var response = new CommentResponse
            {
                Id = comment.Id,
                AuthorId = comment.AuthorId,
                PostId = comment.PostId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };

            return CreatedAtAction(nameof(GetComments), new { commentId = comment.Id }, response);
        }

        // PUT: comments/{commentId}
        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] UpdateCommentByUserRequest request)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (updatedComment, error) = await _commentService.UpdateCommentAsync(commentId, userId.Value, request.Content);
            if (!string.IsNullOrEmpty(error) || updatedComment == null) return BadRequest(new { Error = error });

            var response = new CommentResponse
            {
                Id = updatedComment.Id,
                AuthorId = updatedComment.AuthorId,
                PostId = updatedComment.PostId,
                Content = updatedComment.Content,
                CreatedAt = updatedComment.CreatedAt,
                UpdatedAt = updatedComment.UpdatedAt
            };

            return Ok(response);
        }

        // DELETE: comments/{commentId}
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (deletedId, error) = await _commentService.DeleteCommentAsync(commentId, userId.Value);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

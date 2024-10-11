using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using SocialNetwork.Api.DTOs;
using System.Data;

namespace SocialNetwork.Api.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        // GET: api/comments
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

        // POST: api/comments
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
        {
            var (comment, error) = await _commentService.CreateCommentAsync(request.RequestingUserId, request.PostId, request.Content);
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

        // PUT: api/comments/{commentId}
        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] UpdateCommentRequest request)
        {
            var (updatedComment, error) = await _commentService.UpdateCommentAsync(commentId, request.RequestingUserId, request.Content);
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

        // DELETE: api/comments/{commentId}
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid commentId, [FromQuery] Guid requestingUserId)
        {
            var (deletedId, error) = await _commentService.DeleteCommentAsync(commentId, requestingUserId);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

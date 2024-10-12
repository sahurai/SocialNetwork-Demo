using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using SocialNetwork.Core.Enums;
using SocialNetwork.API.DTO.Post;

namespace SocialNetwork.API.Areas.Admin.Controllers.Post
{
    [ApiController]
    [Area("Admin")]
    [Route("admin/comments")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        // GET: admin/comments
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

        // POST: admin/comments
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
        {
            var (comment, error) = await _commentService.CreateCommentAsync(request.UserId, request.PostId, request.Content);
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

        // PUT: admin/comments/{commentId}
        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] UpdateCommentRequest request)
        {
            var (updatedComment, error) = await _commentService.UpdateCommentAsync(commentId, request.UserId, request.Content);
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

        // DELETE: admin/comments/{commentId}
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid commentId, [FromQuery] Guid authorId)
        {
            var (deletedId, error) = await _commentService.DeleteCommentAsync(commentId, authorId);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

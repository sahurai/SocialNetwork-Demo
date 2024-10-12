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
    [Route("admin/likes")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class LikesController : ControllerBase
    {
        private readonly ILikeService _likeService;
        private readonly ILogger<LikesController> _logger;

        public LikesController(ILikeService likeService, ILogger<LikesController> logger)
        {
            _likeService = likeService;
            _logger = logger;
        }

        // GET: admin/likes
        [HttpGet]
        public async Task<IActionResult> GetLikes([FromQuery] Guid? likeId, [FromQuery] Guid? userId, [FromQuery] Guid? postId, [FromQuery] Guid? commentId)
        {
            var (likes, error) = await _likeService.GetLikesAsync(likeId, userId, postId, commentId);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Error = error });

            var response = likes.Select(like => new LikeResponse
            {
                Id = like.Id,
                UserId = like.UserId,
                PostId = like.PostId,
                CommentId = like.CommentId,
                CreatedAt = like.CreatedAt
            }).ToList();

            return Ok(response);
        }

        // POST: admin/likes
        [HttpPost]
        public async Task<IActionResult> CreateLike([FromBody] CreateLikeRequest request)
        {
            var (like, error) = await _likeService.CreateLikeAsync(request.UserId, request.PostId, request.CommentId);
            if (!string.IsNullOrEmpty(error) || like == null) return BadRequest(new { Error = error });

            var response = new LikeResponse
            {
                Id = like.Id,
                UserId = like.UserId,
                PostId = like.PostId,
                CommentId = like.CommentId,
                CreatedAt = like.CreatedAt
            };

            return CreatedAtAction(nameof(GetLikes), new { likeId = like.Id }, response);
        }

        // DELETE: admin/likes/{likeId}
        [HttpDelete("{likeId}")]
        public async Task<IActionResult> DeleteLike(Guid likeId, [FromQuery] Guid userId)
        {
            var (deletedId, error) = await _likeService.DeleteLikeAsync(likeId, userId);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

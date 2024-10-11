using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using SocialNetwork.Api.DTOs;
using System.Data;

namespace SocialNetwork.Api.Controllers
{
    [ApiController]
    [Route("api/likes")]
    public class LikesController : ControllerBase
    {
        private readonly ILikeService _likeService;
        private readonly ILogger<LikesController> _logger;

        public LikesController(ILikeService likeService, ILogger<LikesController> logger)
        {
            _likeService = likeService;
            _logger = logger;
        }

        // GET: api/likes
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

        // POST: api/likes
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

        // DELETE: api/likes/{likeId}
        [HttpDelete("{likeId}")]
        public async Task<IActionResult> DeleteLike(Guid likeId, [FromQuery] Guid requestingUserId)
        {
            var (deletedId, error) = await _likeService.DeleteLikeAsync(likeId, requestingUserId);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

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
    [Route("likes")]
    [ApiExplorerSettings(GroupName = "User")]
    public class LikesController : ControllerBase
    {
        private readonly ILikeService _likeService;
        private readonly ILogger<LikesController> _logger;

        public LikesController(ILikeService likeService, ILogger<LikesController> logger)
        {
            _likeService = likeService;
            _logger = logger;
        }

        // GET: likes
        [HttpGet]
        public async Task<IActionResult> GetLikes([FromQuery] Guid? likeId, [FromQuery] Guid? postId, [FromQuery] Guid? commentId)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (likes, error) = await _likeService.GetLikesAsync(likeId, userId.Value, postId, commentId);
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

        // POST: likes
        [HttpPost]
        public async Task<IActionResult> CreateLike([FromBody] CreateLikeByUserRequest request)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (like, error) = await _likeService.CreateLikeAsync(userId.Value, request.PostId, request.CommentId);
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

        // DELETE: likes/{likeId}
        [HttpDelete("{likeId}")]
        public async Task<IActionResult> DeleteLike(Guid likeId)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (deletedId, error) = await _likeService.DeleteLikeAsync(likeId, userId.Value);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

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
    [Route("admin/posts")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostsController> _logger;

        public PostsController(IPostService postService, ILogger<PostsController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        // GET: admin/posts
        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] Guid? postId, [FromQuery] Guid? authorId, [FromQuery] Guid? groupId, [FromQuery] string? content)
        {
            var (posts, error) = await _postService.GetPostsAsync(postId, authorId, groupId, content);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Error = error });

            var response = posts.Select(post => new PostResponse
            {
                Id = post.Id,
                AuthorId = post.AuthorId,
                GroupId = post.GroupId,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            }).ToList();

            return Ok(response);
        }

        // POST: admin/posts
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {
            var (post, error) = await _postService.CreatePostAsync(request.UserId, request.Content, request.GroupId);
            if (!string.IsNullOrEmpty(error) || post == null) return BadRequest(new { Error = error });

            var response = new PostResponse
            {
                Id = post.Id,
                AuthorId = post.AuthorId,
                GroupId = post.GroupId,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            };

            return CreatedAtAction(nameof(GetPosts), new { postId = post.Id }, response);
        }

        // PUT: admin/posts/{postId}
        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(Guid postId, [FromBody] UpdatePostRequest request)
        {
            var (updatedPost, error) = await _postService.UpdatePostAsync(postId, request.UserId, request.Content);
            if (!string.IsNullOrEmpty(error) || updatedPost == null) return BadRequest(new { Error = error });

            var response = new PostResponse
            {
                Id = updatedPost.Id,
                AuthorId = updatedPost.AuthorId,
                GroupId = updatedPost.GroupId,
                Content = updatedPost.Content,
                CreatedAt = updatedPost.CreatedAt,
                UpdatedAt = updatedPost.UpdatedAt
            };

            return Ok(response);
        }

        // DELETE: admin/posts/{postId}
        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(Guid postId, [FromQuery] Guid authorId)
        {
            var (deletedId, error) = await _postService.DeletePostAsync(postId, authorId);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }
    }
}

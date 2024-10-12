using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using System.Data;
using SocialNetwork.API.DTO.User;
using Microsoft.AspNetCore.Authorization;
using SocialNetwork.Shared;

namespace SocialNetwork.API.Areas.User.Controllers.User
{
    [ApiController]
    [Authorize]
    [Area("User")]
    [Route("messages")]
    [ApiExplorerSettings(GroupName = "User")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(IMessageService messageService, ILogger<MessagesController> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        // GET: messages?messageId=GUID&receiverId=GUID
        [HttpGet]
        public async Task<IActionResult> GetMessages([FromQuery] Guid? messageId, [FromQuery] Guid? receiverId)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (messages, error) = await _messageService.GetMessagesAsync(messageId, userId.Value, receiverId);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Error = error });

            var response = messages.Select(message => new MessageResponse
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                IsRead = message.IsRead,
                CreatedAt = message.CreatedAt,
                UpdatedAt = message.UpdatedAt
            }).ToList();

            return Ok(response);
        }

        // GET: messages/conversation?otherUserId=GUID
        [HttpGet("conversation")]
        public async Task<IActionResult> GetConversation([FromQuery] Guid otherUserId)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (messages, error) = await _messageService.GetConversationAsync(userId.Value, otherUserId);
            if (!string.IsNullOrEmpty(error)) return BadRequest(new { Error = error });

            var response = messages.Select(message => new MessageResponse
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                IsRead = message.IsRead,
                CreatedAt = message.CreatedAt,
                UpdatedAt = message.UpdatedAt
            }).ToList();

            return Ok(response);
        }

        // POST: messages
        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageByUserRequest request)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (message, error) = await _messageService.CreateMessageAsync(userId.Value, request.ReceiverId, request.Content);
            if (!string.IsNullOrEmpty(error) || message == null) return BadRequest(new { Error = error });

            var response = new MessageResponse
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                IsRead = message.IsRead,
                CreatedAt = message.CreatedAt,
                UpdatedAt = message.UpdatedAt
            };

            return CreatedAtAction(nameof(GetMessages), new { messageId = message.Id }, response);
        }

        // PUT: messages/{messageId}
        [HttpPut("{messageId}")]
        public async Task<IActionResult> UpdateMessage(Guid messageId, [FromBody] UpdateMessageByUserRequest request)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (updatedMessage, error) = await _messageService.UpdateMessageAsync(messageId, userId.Value, request.Content);
            if (!string.IsNullOrEmpty(error) || updatedMessage == null) return BadRequest(new { Error = error });

            var response = new MessageResponse
            {
                Id = updatedMessage.Id,
                SenderId = updatedMessage.SenderId,
                ReceiverId = updatedMessage.ReceiverId,
                Content = updatedMessage.Content,
                IsRead = updatedMessage.IsRead,
                CreatedAt = updatedMessage.CreatedAt,
                UpdatedAt = updatedMessage.UpdatedAt
            };

            return Ok(response);
        }

        // POST: messages/mark-as-read
        [HttpPost("mark-as-read")]
        public async Task<IActionResult> MarkMessagesAsRead([FromBody] MarkMessagesAsReadByUserRequest request)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (success, error) = await _messageService.MarkMessagesAsReadAsync(userId.Value, request.MessageIds);
            if (!success) return BadRequest(new { Error = error });

            return Ok("Messages marked as read.");
        }

        // DELETE: messages/{messageId}
        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (deletedId, error) = await _messageService.DeleteMessageAsync(messageId, userId.Value);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }

        // DELETE: messages/conversation?otherUserId=GUID
        [HttpDelete("conversation")]
        public async Task<IActionResult> DeleteConversation([FromQuery] Guid otherUserId)
        {
            var (userId, extractError) = Utilities.ExtractUserIdFromClaimsPrincipal(User);
            if (!string.IsNullOrEmpty(extractError) || userId == null) return Unauthorized(new { error = extractError });

            var (success, error) = await _messageService.DeleteConversationAsync(userId.Value, otherUserId);
            if (!success) return BadRequest(new { Error = error });

            return Ok("Conversation deleted successfully.");
        }
    }
}

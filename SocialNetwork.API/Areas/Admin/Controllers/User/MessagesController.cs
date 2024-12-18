﻿using Microsoft.AspNetCore.Mvc;
using SocialNetwork.ApplicationLogic.Services;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using SocialNetwork.Core.Enums;
using SocialNetwork.API.DTO.User;

namespace SocialNetwork.API.Areas.Admin.Controllers.User
{
    [ApiController]
    [Area("Admin")]
    [Route("admin/messages")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(IMessageService messageService, ILogger<MessagesController> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        // GET: admin/messages?messageId=GUID&senderId=GUID&receiverId=GUID
        [HttpGet]
        public async Task<IActionResult> GetMessages([FromQuery] Guid? messageId, [FromQuery] Guid? senderId, [FromQuery] Guid? receiverId)
        {
            var (messages, error) = await _messageService.GetMessagesAsync(messageId, senderId, receiverId);
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

        // GET: admin/messages/conversation?userId=GUID&otherUserId=GUID
        [HttpGet("conversation")]
        public async Task<IActionResult> GetConversation([FromQuery] Guid userId, [FromQuery] Guid otherUserId)
        {
            var (messages, error) = await _messageService.GetConversationAsync(userId, otherUserId);
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

        // POST: admin/messages
        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageRequest request)
        {
            var (message, error) = await _messageService.CreateMessageAsync(request.SenderId, request.ReceiverId, request.Content);
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

        // PUT: admin/messages/{messageId}
        [HttpPut("{messageId}")]
        public async Task<IActionResult> UpdateMessage(Guid messageId, [FromBody] UpdateMessageRequest request)
        {
            var (updatedMessage, error) = await _messageService.UpdateMessageAsync(messageId, request.SenderId, request.Content);
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

        // POST: admin/messages/mark-as-read
        [HttpPost("mark-as-read")]
        public async Task<IActionResult> MarkMessagesAsRead([FromBody] MarkMessagesAsReadRequest request)
        {
            var (success, error) = await _messageService.MarkMessagesAsReadAsync(request.UserId, request.MessageIds);
            if (!success) return BadRequest(new { Error = error });

            return Ok("Messages marked as read.");
        }

        // DELETE: admin/messages/{messageId}?userId=GUID
        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(Guid messageId, [FromQuery] Guid userId)
        {
            var (deletedId, error) = await _messageService.DeleteMessageAsync(messageId, userId);
            if (deletedId == Guid.Empty) return BadRequest(new { Error = error });

            return Ok(new { DeletedId = deletedId });
        }

        // DELETE: admin/messages/conversation?userId=GUID&otherUserId=GUID
        [HttpDelete("conversation")]
        public async Task<IActionResult> DeleteConversation([FromQuery] Guid userId, [FromQuery] Guid otherUserId)
        {
            var (success, error) = await _messageService.DeleteConversationAsync(userId, otherUserId);
            if (!success) return BadRequest(new { Error = error });

            return Ok("Conversation deleted successfully.");
        }
    }
}

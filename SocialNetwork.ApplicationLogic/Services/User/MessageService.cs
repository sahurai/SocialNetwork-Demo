using Microsoft.Extensions.Logging;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Repositories;

namespace SocialNetwork.ApplicationLogic.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<MessageService> _logger;

        public MessageService(IMessageRepository messageRepository, ILogger<MessageService> logger)
        {
            _messageRepository = messageRepository;
            _logger = logger;
        }

        // Retrieve messages with optional filtering
        public async Task<(List<Message> Messages, string Error)> GetMessagesAsync(Guid? messageId = null, Guid? senderId = null, Guid? receiverId = null)
        {
            try
            {
                var messages = await _messageRepository.GetAsync(messageId, senderId, receiverId);
                return (messages, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving messages.");
                return (new List<Message>(), "An error occurred while retrieving messages.");
            }
        }

        // Retrieve conversation between two users
        public async Task<(List<Message> Messages, string Error)> GetConversationAsync(Guid requestingUserId, Guid otherUserId)
        {
            try
            {
                // Retrieve the conversation
                var messages = await _messageRepository.GetConversationAsync(requestingUserId, otherUserId);
                return (messages, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the conversation between {requestingUserId} and {otherUserId}.");
                return (new List<Message>(), "An error occurred while retrieving the conversation.");
            }
        }

        // Create a new message
        public async Task<(Message? Message, string Error)> CreateMessageAsync(Guid requestingUserId, Guid receiverId, string content)
        {
            try
            {
                // Create the message model
                var (message, createError) = Message.Create(requestingUserId, receiverId, content);
                if (message == null) return (null, createError);

                // Save to the database
                var createdMessage = await _messageRepository.CreateAsync(message);
                return (createdMessage, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a message.");
                return (null, "An error occurred while creating the message.");
            }
        }

        // Update an existing message
        public async Task<(Message? Message, string Error)> UpdateMessageAsync(Guid messageId, Guid senderId, string content)
        {
            try
            {
                // Retrieve the message
                var messages = await _messageRepository.GetAsync(messageId: messageId);
                var message = messages.FirstOrDefault();
                if (message == null) return (null, "Message not found.");

                // Ensure the sender is the same
                if (message.SenderId != senderId) return (null, "You can only update your own messages.");

                // Edit the content
                var editError = message.EditContent(content);
                if (!string.IsNullOrEmpty(editError)) return (null, editError);

                // Update in the database
                var result = await _messageRepository.UpdateAsync(messageId, message);
                return (result, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the message.");
                return (null, "An error occurred while updating the message.");
            }
        }

        // Mark multiple messages as read
        public async Task<(bool Success, string Error)> MarkMessagesAsReadAsync(Guid requestingUserId, List<Guid> messageIds)
        {
            try
            {
                // Retrieve the messages
                var messages = await _messageRepository.GetAsync();
                var messagesToUpdate = messages.Where(m => messageIds.Contains(m.Id) && m.ReceiverId == requestingUserId && !m.IsRead).ToList();

                if (!messagesToUpdate.Any())
                {
                    return (false, "No messages found to mark as read.");
                }

                // Mark each message as read
                foreach (var message in messagesToUpdate)
                {
                    message.MarkAsRead();
                }

                // Update messages in the database using the new repository method
                await _messageRepository.UpdateMultipleAsync(messagesToUpdate);

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking messages as read.");
                return (false, "An error occurred while marking the messages as read.");
            }
        }

        // Delete a message
        public async Task<(Guid Id, string Error)> DeleteMessageAsync(Guid messageId, Guid requestingUserId)
        {
            try
            {
                // Retrieve the message
                var messages = await _messageRepository.GetAsync(messageId: messageId);
                var message = messages.FirstOrDefault();
                if (message == null) return (Guid.Empty, "Message not found.");

                // Check if the requesting user is the sender
                if (message.SenderId != requestingUserId) return (Guid.Empty, "You can only delete your own messages.");

                // Delete from the database
                var deletedId = await _messageRepository.DeleteAsync(messageId);
                return (deletedId, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the message.");
                return (Guid.Empty, "An error occurred while deleting the message.");
            }
        }

        // Delete an entire conversation between two users
        public async Task<(bool Success, string Error)> DeleteConversationAsync(Guid requestingUserId, Guid otherUserId)
        {
            try
            {
                // Delete the conversation
                await _messageRepository.DeleteConversationAsync(requestingUserId, otherUserId);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the conversation between {requestingUserId} and {otherUserId}.");
                return (false, $"An error occurred while deleting the conversation between {requestingUserId} and {otherUserId}.");
            }
        }
    }
}

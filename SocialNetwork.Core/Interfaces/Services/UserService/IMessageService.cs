using SocialNetwork.Core.Models;

namespace SocialNetwork.ApplicationLogic.Services
{
    public interface IMessageService
    {
        Task<(Message? Message, string Error)> CreateMessageAsync(Guid senderId, Guid receiverId, string content);
        Task<(Guid Id, string Error)> DeleteMessageAsync(Guid messageId, Guid requestingUserId);
        Task<(bool Success, string Error)> DeleteConversationAsync(Guid requestingUserId, Guid otherUserId);
        Task<(List<Message> Messages, string Error)> GetMessagesAsync(Guid? messageId = null, Guid? senderId = null, Guid? receiverId = null);
        Task<(List<Message> Messages, string Error)> GetConversationAsync(Guid requestingUserId, Guid otherUserId);
        Task<(Message? Message, string Error)> UpdateMessageAsync(Guid id, Guid senderId, string content);
        Task<(bool Success, string Error)> MarkMessagesAsReadAsync(Guid requestingUserId, List<Guid> messageIds);
    }
}
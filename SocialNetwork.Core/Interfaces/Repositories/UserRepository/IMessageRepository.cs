using SocialNetwork.Core.Models;

namespace SocialNetwork.DataAccess.Repositories
{
    public interface IMessageRepository
    {
        Task<Message> CreateAsync(Message message);
        Task<Guid> DeleteAsync(Guid id);
        Task<bool> DeleteConversationAsync(Guid userId1, Guid userId2);
        Task<List<Message>> GetAsync(Guid? messageId = null, Guid? senderId = null, Guid? receiverId = null);
        Task<List<Message>> GetConversationAsync(Guid userId1, Guid userId2);
        Task<Message> UpdateAsync(Guid id, Message updatedMessage);
        Task<List<Message>> UpdateMultipleAsync(List<Message> updatedMessages);
    }
}
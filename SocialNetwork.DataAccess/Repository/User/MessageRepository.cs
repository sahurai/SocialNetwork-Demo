using Microsoft.EntityFrameworkCore;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Entities.User;

namespace SocialNetwork.DataAccess.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly SocialNetworkDbContext _context;

        public MessageRepository(SocialNetworkDbContext context)
        {
            _context = context;
        }

        // Retrieve messages with optional filtering
        public async Task<List<Message>> GetAsync(Guid? messageId = null, Guid? senderId = null, Guid? receiverId = null)
        {
            IQueryable<MessageEntity> query = _context.Messages.AsNoTracking();

            if (messageId.HasValue)
            {
                query = query.Where(message => message.Id == messageId.Value);
            }

            if (senderId.HasValue)
            {
                query = query.Where(message => message.SenderId == senderId.Value);
            }

            if (receiverId.HasValue)
            {
                query = query.Where(message => message.ReceiverId == receiverId.Value);
            }

            query = query.OrderByDescending(message => message.CreatedAt);

            List<MessageEntity> messageEntities = await query.ToListAsync();

            List<Message> messages = messageEntities.Select(MapToModel).ToList();

            return messages;
        }

        // Retrive conversation between two users
        public async Task<List<Message>> GetConversationAsync(Guid userId1, Guid userId2)
        {
            IQueryable<MessageEntity> query = _context.Messages.AsNoTracking()
                .Where(message =>
                    (message.SenderId == userId1 && message.ReceiverId == userId2) ||
                    (message.SenderId == userId2 && message.ReceiverId == userId1))
                .OrderBy(message => message.CreatedAt);

            List<MessageEntity> messageEntities = await query.ToListAsync();

            List<Message> messages = messageEntities.Select(MapToModel).ToList();

            return messages;
        }

        // Create a new message
        public async Task<Message> CreateAsync(Message message)
        {
            MessageEntity messageEntity = new MessageEntity
            {
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                IsRead = message.IsRead
            };

            await _context.Messages.AddAsync(messageEntity);
            await _context.SaveChangesAsync();

            return MapToModel(messageEntity);
        }

        // Update an existing message
        public async Task<Message> UpdateAsync(Guid id, Message updatedMessage)
        {
            await _context.Messages
                .Where(message => message.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(message => message.Content, updatedMessage.Content)
                    .SetProperty(message => message.EditedAt, updatedMessage.EditedAt)
                    .SetProperty(message => message.IsRead, updatedMessage.IsRead)
                    .SetProperty(message => message.UpdatedAt, updatedMessage.UpdatedAt));

            return updatedMessage;
        }

        // Update multiple existing messages in one transaction
        public async Task<List<Message>> UpdateMultipleAsync(List<Message> updatedMessages)
        {
            foreach (var updatedMessage in updatedMessages)
            {
                _context.Messages.Update(MapToEntity(updatedMessage));
            }

            await _context.SaveChangesAsync();

            return updatedMessages;
        }

        // Delete a message by Id
        public async Task<Guid> DeleteAsync(Guid id)
        {
            await _context.Messages
                .Where(message => message.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }

        // Delete whole conversation
        public async Task<bool> DeleteConversationAsync(Guid userId1, Guid userId2)
        {
            IQueryable<MessageEntity> query = _context.Messages
                .Where(message =>
                    (message.SenderId == userId1 && message.ReceiverId == userId2) ||
                    (message.SenderId == userId2 && message.ReceiverId == userId1));

            _context.Messages.RemoveRange(query);

            await _context.SaveChangesAsync();

            return true;
        }

        // Map MessageEntity to Message model
        private Message MapToModel(MessageEntity entity)
        {
            var (message, error) = Message.CreateFromDb(
                entity.Id,
                entity.SenderId,
                entity.ReceiverId,
                entity.Content,
                entity.EditedAt,
                entity.IsRead,
                entity.CreatedAt,
                entity.UpdatedAt);

            if (!string.IsNullOrEmpty(error)) throw new Exception($"Error during mapping: {error}");

            return message!;
        }

        // Map Message model to MessageEntity
        private MessageEntity MapToEntity(Message message)
        {
            return new MessageEntity
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                EditedAt = message.EditedAt,
                IsRead = message.IsRead,
                CreatedAt = message.CreatedAt,
                UpdatedAt = message.UpdatedAt
            };
        }
    }
}

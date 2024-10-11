using System;
using SocialNetwork.Core.Validators;

namespace SocialNetwork.Core.Models
{
    /// <summary>
    /// Represents a private message between two users.
    /// </summary>
    public class Message : BaseClass
    {
        public string Content { get; private set; }
        public Guid SenderId { get; }
        public Guid ReceiverId { get; }
        public DateTime? EditedAt { get; private set; }
        public bool IsRead { get; private set; }

        // Private constructor
        private Message(Guid id, Guid senderId, Guid receiverId, string content)
        {
            Id = id;
            SenderId = senderId;
            ReceiverId = receiverId;
            Content = content;
            IsRead = false;
        }

        // Static method to create a new Message with validation
        public static (Message? Message, string Error) Create(
            Guid senderId,
            Guid receiverId,
            string content)
        {
            var message = new Message(Guid.NewGuid(), senderId, receiverId, content);

            // Validate the instance
            var validator = new MessageValidator();
            var validationResult = validator.Validate(message);

            if (!validationResult.IsValid)
            {
                string error = string.Join("; ", validationResult.Errors);
                return (null, error);
            }

            return (message, string.Empty);
        }

        // Method to mark the message as read
        public void MarkAsRead()
        {
            IsRead = true;
            UpdatedAt = DateTime.UtcNow;
        }

        // Method to edit the message content
        public string EditContent(string newContent)
        {
            Content = newContent;
            EditedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            // Validate the new content
            var validator = new MessageValidator();
            var validationResult = validator.Validate(this);

            if (!validationResult.IsValid)
            {
                return string.Join("; ", validationResult.Errors);
            }

            return string.Empty;
        }

        // Method to recreate an instance from database data
        public static (Message? Message, string Error) CreateFromDb(
            Guid id,
            Guid senderId,
            Guid receiverId,
            string content,
            DateTime? editedAt,
            bool isRead,
            DateTime createdAt,
            DateTime updatedAt)
        {
            var message = new Message(id, senderId, receiverId, content)
            {
                EditedAt = editedAt,
                IsRead = isRead,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            return (message, string.Empty);
        }
    }
}

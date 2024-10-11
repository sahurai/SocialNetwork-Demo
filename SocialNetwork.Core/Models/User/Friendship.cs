using System;
using SocialNetwork.Core.Validators;

namespace SocialNetwork.Core.Models
{
    /// <summary>
    /// Represents a friendship relationship between two users.
    /// </summary>
    public class Friendship : BaseClass
    {
        public Guid User1Id { get; }
        public Guid User2Id { get; }
        public Guid RequestedById { get; }
        public DateTime? AcceptedAt { get; private set; }

        // Private constructor
        private Friendship(Guid id, Guid user1Id, Guid user2Id, Guid requestedById, DateTime? acceptedAt)
        {
            Id = id;
            User1Id = user1Id;
            User2Id = user2Id;
            RequestedById = requestedById;
            AcceptedAt = acceptedAt;
        }

        // Static method to create a new Friendship with validation
        public static (Friendship? Friendship, string Error) Create(
            Guid requestingUserId,
            Guid user2Id)
        {
            var friendship = new Friendship(Guid.NewGuid(), requestingUserId, user2Id, requestingUserId, null);

            // Validate the instance
            var validator = new FriendshipValidator();
            var validationResult = validator.Validate(friendship);

            if (!validationResult.IsValid)
            {
                string error = string.Join("; ", validationResult.Errors);
                return (null, error);
            }

            return (friendship, string.Empty);
        }

        // Method to accept the friendship
        public void Accept()
        {
            if (AcceptedAt != null)
            {
                throw new InvalidOperationException("Friendship has already been accepted.");
            }

            AcceptedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        // Method to recreate an instance from database data
        public static (Friendship? Friendship, string Error) CreateFromDb(
            Guid id,
            Guid user1Id,
            Guid user2Id,
            Guid requestedById,
            DateTime? acceptedAt,
            DateTime createdAt,
            DateTime updatedAt)
        {
            var friendship = new Friendship(id, user1Id, user2Id, requestedById, acceptedAt)
            {
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            return (friendship, string.Empty);
        }
    }
}

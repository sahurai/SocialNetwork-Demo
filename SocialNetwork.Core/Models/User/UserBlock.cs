using SocialNetwork.Core.Validators;

namespace SocialNetwork.Core.Models
{
    /// <summary>
    /// Represents a block relationship between two users.
    /// </summary>
    public class UserBlock : BaseClass
    {
        public Guid BlockerId { get; }
        public Guid BlockedId { get; }

        // Private constructor
        private UserBlock(Guid id, Guid blockerId, Guid blockedId)
        {
            Id = id;
            BlockerId = blockerId;
            BlockedId = blockedId;
        }

        // Static method to create a new UserBlock with validation
        public static (UserBlock? UserBlock, string Error) Create(
            Guid blockerId,
            Guid blockedId)
        {
            var userBlock = new UserBlock(Guid.NewGuid(), blockerId, blockedId);

            // Validate the instance
            var validator = new UserBlockValidator();
            var validationResult = validator.Validate(userBlock);

            if (!validationResult.IsValid)
            {
                string error = string.Join("; ", validationResult.Errors);
                return (null, error);
            }

            return (userBlock, string.Empty);
        }

        // Method to recreate an instance from database data
        public static (UserBlock? UserBlock, string Error) CreateFromDb(
            Guid id,
            Guid blockerId,
            Guid blockedId,
            DateTime createdAt,
            DateTime updatedAt)
        {
            var userBlock = new UserBlock(id, blockerId, blockedId)
            {
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            return (userBlock, string.Empty);
        }
    }
}

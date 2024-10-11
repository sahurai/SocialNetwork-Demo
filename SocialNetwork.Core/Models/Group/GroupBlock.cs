using System;
using SocialNetwork.Core.Validators;

namespace SocialNetwork.Core.Models
{
    /// <summary>
    /// Represents a block relationship between two users within a group.
    /// </summary>
    public class GroupBlock : BaseClass
    {
        public Guid BlockerId { get; }
        public Guid BlockedId { get; }
        public Guid GroupId { get; }

        // Private constructor to create a GroupBlock instance
        private GroupBlock(Guid id, Guid blockerId, Guid blockedId, Guid groupId)
        {
            Id = id;
            BlockerId = blockerId;
            BlockedId = blockedId;
            GroupId = groupId;
        }

        // Static method to create a new GroupBlock with validation
        public static (GroupBlock? GroupBlock, string Error) Create(
            Guid blockerId,
            Guid blockedId,
            Guid groupId)
        {
            var groupBlock = new GroupBlock(Guid.NewGuid(), blockerId, blockedId, groupId);

            // Validate the instance
            var validator = new GroupBlockValidator();
            var validationResult = validator.Validate(groupBlock);

            if (!validationResult.IsValid)
            {
                string error = string.Join("; ", validationResult.Errors);
                return (null, error);
            }

            return (groupBlock, string.Empty);
        }

        // Method to recreate an instance from database data
        public static (GroupBlock? GroupBlock, string Error) CreateFromDb(
            Guid id,
            Guid blockerId,
            Guid blockedId,
            Guid groupId,
            DateTime createdAt,
            DateTime updatedAt)
        {
            var groupBlock = new GroupBlock(id, blockerId, blockedId, groupId)
            {
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            return (groupBlock, string.Empty);
        }
    }
}

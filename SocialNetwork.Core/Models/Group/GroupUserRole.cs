using System;
using SocialNetwork.Core.Enums;
using SocialNetwork.Core.Validators;

namespace SocialNetwork.Core.Models
{
    /// <summary>
    /// Represents the role of a user in a group (Admin, Manager, Member).
    /// </summary>
    public class GroupUserRole : BaseClass
    {
        public Guid UserId { get; }
        public Guid GroupId { get; }
        public GroupRole Role { get; }

        // Private constructor
        private GroupUserRole(Guid id, Guid userId, Guid groupId, GroupRole role)
        {
            Id = id;
            UserId = userId;
            GroupId = groupId;
            Role = role;
        }

        // Static method to create a new GroupUserRole with validation
        public static (GroupUserRole? GroupUserRole, string Error) Create(
            Guid userId,
            Guid groupId,
            GroupRole role)
        {
            var groupUserRole = new GroupUserRole(Guid.NewGuid(), userId, groupId, role);

            // Validate the instance
            var validator = new GroupUserRoleValidator();
            var validationResult = validator.Validate(groupUserRole);

            if (!validationResult.IsValid)
            {
                string error = string.Join("; ", validationResult.Errors);
                return (null, error);
            }

            return (groupUserRole, string.Empty);
        }

        // Method to recreate an instance from database data
        public static (GroupUserRole? GroupUserRole, string Error) CreateFromDb(
            Guid id,
            Guid userId,
            Guid groupId,
            GroupRole role,
            DateTime createdAt,
            DateTime updatedAt)
        {
            var groupUserRole = new GroupUserRole(id, userId, groupId, role)
            {
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            return (groupUserRole, string.Empty);
        }
    }
}

using System;
using SocialNetwork.Core.Validators;

namespace SocialNetwork.Core.Models
{
    /// <summary>
    /// Represents a like on a post or comment.
    /// </summary>
    public class Like : BaseClass
    {
        public Guid UserId { get; }
        public Guid? PostId { get; }
        public Guid? CommentId { get; }

        // Private constructor
        private Like(Guid id, Guid userId, Guid? postId, Guid? commentId)
        {
            Id = id;
            UserId = userId;
            PostId = postId;
            CommentId = commentId;
        }

        // Static method to create a new Like with validation
        public static (Like? Like, string Error) Create(
            Guid userId,
            Guid? postId,
            Guid? commentId)
        {
            var like = new Like(Guid.NewGuid(), userId, postId, commentId);

            // Validate the instance
            var validator = new LikeValidator();
            var validationResult = validator.Validate(like);

            if (!validationResult.IsValid)
            {
                string error = string.Join("; ", validationResult.Errors);
                return (null, error);
            }

            return (like, string.Empty);
        }

        // Method to recreate an instance from database data
        public static (Like? Like, string Error) CreateFromDb(
            Guid id,
            Guid userId,
            Guid? postId,
            Guid? commentId,
            DateTime createdAt,
            DateTime updatedAt)
        {
            var like = new Like(id, userId, postId, commentId)
            {
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            return (like, string.Empty);
        }
    }
}

using System;
using SocialNetwork.Core.Validators;

namespace SocialNetwork.Core.Models
{
    /// <summary>
    /// Represents a comment made on a post.
    /// </summary>
    public class Comment : BaseClass
    {
        public string Content { get; private set; }
        public Guid AuthorId { get; }
        public Guid PostId { get; }
        public DateTime? EditedAt { get; private set; }

        // Private constructor
        private Comment(Guid id, Guid authorId, Guid postId, string content, DateTime? editedAt)
        {
            Id = id;
            AuthorId = authorId;
            PostId = postId;
            Content = content;
            EditedAt = editedAt;
        }

        // Static method to create a new Comment with validation
        public static (Comment? Comment, string Error) Create(
            Guid authorId,
            Guid postId,
            string content)
        {
            var comment = new Comment(Guid.NewGuid(), authorId, postId, content, null);

            // Validate the instance
            var validator = new CommentValidator();
            var validationResult = validator.Validate(comment);

            if (!validationResult.IsValid)
            {
                string error = string.Join("; ", validationResult.Errors);
                return (null, error);
            }

            return (comment, string.Empty);
        }

        // Method to edit the comment content
        public string EditContent(string newContent)
        {
            Content = newContent;
            EditedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            // Validate the new content
            var validator = new CommentValidator();
            var validationResult = validator.Validate(this);

            if (!validationResult.IsValid)
            {
                return string.Join("; ", validationResult.Errors);
            }

            return string.Empty;
        }

        // Method to recreate an instance from database data
        public static (Comment? Comment, string Error) CreateFromDb(
            Guid id,
            Guid authorId,
            Guid postId,
            string content,
            DateTime? editedAt,
            DateTime createdAt,
            DateTime updatedAt)
        {
            var comment = new Comment(id, authorId, postId, content, editedAt)
            {
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            return (comment, string.Empty);
        }
    }
}

using System;
using System.Collections.Generic;
using SocialNetwork.Core.Validators;

namespace SocialNetwork.Core.Models
{
    /// <summary>
    /// Represents a post made by a user or in a group.
    /// </summary>
    public class Post : BaseClass
    {
        public string Content { get; private set; }
        public Guid AuthorId { get; }
        public Guid? GroupId { get; }
        public ICollection<Comment> Comments { get; } = new List<Comment>();
        public ICollection<Like> Likes { get; } = new List<Like>();
        public DateTime? EditedAt { get; private set; }

        // Private constructor
        private Post(Guid id, Guid authorId, string content, Guid? groupId)
        {
            Id = id;
            AuthorId = authorId;
            Content = content;
            GroupId = groupId;
        }

        // Static method to create a new Post with validation
        public static (Post? Post, string Error) Create(
            Guid authorId,
            string content,
            Guid? groupId)
        {
            var post = new Post(Guid.NewGuid(), authorId, content, groupId);

            // Validate the instance
            var validator = new PostValidator();
            var validationResult = validator.Validate(post);

            if (!validationResult.IsValid)
            {
                string error = string.Join("; ", validationResult.Errors);
                return (null, error);
            }

            return (post, string.Empty);
        }

        // Method to edit the post content
        public string EditContent(string newContent)
        {
            Content = newContent;
            EditedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            // Validate the new content
            var validator = new PostValidator();
            var validationResult = validator.Validate(this);

            if (!validationResult.IsValid)
            {
                return string.Join("; ", validationResult.Errors);
            }

            return string.Empty;
        }

        // Method to recreate an instance from database data
        public static (Post? Post, string Error) CreateFromDb(
            Guid id,
            Guid authorId,
            string content,
            Guid? groupId,
            DateTime? editedAt,
            DateTime createdAt,
            DateTime updatedAt)
        {
            var post = new Post(id, authorId, content, groupId)
            {
                EditedAt = editedAt,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            return (post, string.Empty);
        }
    }
}

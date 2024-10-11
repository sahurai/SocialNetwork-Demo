using SocialNetwork.Core.Validators;

namespace SocialNetwork.Core.Models
{
    /// <summary>
    /// Represents a group that users can join.
    /// </summary>
    public class Group : BaseClass
    {
        public Guid CreatorId { get; }
        public string Name { get; }
        public string? Description { get; }
        public ICollection<GroupUserRole> Members { get; } = new List<GroupUserRole>();

        // Private constructor to create a Group instance
        private Group(Guid id, Guid creatorId, string name, string? description)
        {
            Id = id;
            CreatorId = creatorId;
            Name = name;
            Description = description;
        }

        // Static method to create a new Group with validation
        public static (Group? Group, string Error) Create(
            Guid creatorId,
            string name,
            string? description)
        {
            var group = new Group(Guid.NewGuid(), creatorId, name, description);

            // Validate the instance
            var validator = new GroupValidator();
            var validationResult = validator.Validate(group);

            if (!validationResult.IsValid)
            {
                string error = string.Join("; ", validationResult.Errors);
                return (null, error);
            }

            return (group, string.Empty);
        }

        // Method to recreate an instance from database data
        public static (Group? Group, string Error) CreateFromDb(
            Guid id,
            Guid creatorId,
            string name,
            string? description,
            DateTime createdAt,
            DateTime updatedAt)
        {
            var group = new Group(id, creatorId, name, description)
            {
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            return (group, string.Empty);
        }
    }
}

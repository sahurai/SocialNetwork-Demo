namespace SocialNetwork.Core.Models
{
    public abstract class BaseClass
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;
    }
}

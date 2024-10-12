using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DataAccess.Entities;
using SocialNetwork.Shared;

namespace SocialNetwork.DataAccess.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            // Primary key for the user
            builder.HasKey(user => user.Id);

            // Username is required with a maximum length constraint
            builder.Property(user => user.Username)
                .HasMaxLength(Constants.MaxUsernameLength)
                .IsRequired();

            // Username must be unique
            builder.HasIndex(user => user.Username)
                .IsUnique();

            // Email is required and must follow the email format
            builder.Property(user => user.Email)
                .IsRequired()
                .HasMaxLength(Constants.MaxEmailLength);

            // Email must be unique
            builder.HasIndex(user => user.Email)
                .IsUnique();

            // User role in the system
            builder.Property(user => user.Role)
                .IsRequired();

            // Password is required
            builder.Property(user => user.PasswordHash)
                .IsRequired();

            // One-to-Many: User -> Posts
            builder.HasMany(user => user.Posts)
                .WithOne(post => post.Author)
                .HasForeignKey(post => post.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: User -> SentMessages
            builder.HasMany(user => user.SentMessages)
                .WithOne(message => message.Sender)
                .HasForeignKey(message => message.SenderId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: User -> ReceivedMessages
            builder.HasMany(user => user.ReceivedMessages)
                .WithOne(message => message.Receiver)
                .HasForeignKey(message => message.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: User -> Friendships
            builder.HasMany(user => user.Friendships)
                .WithOne(friendship => friendship.User1)
                .HasForeignKey(friendship => friendship.User1Id)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: User -> BlockedUsers
            builder.HasMany(user => user.BlockedUsers)
                .WithOne(block => block.Blocker)
                .HasForeignKey(block => block.BlockerId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: User -> RefreshTokens
            // A user can have multiple refresh tokens
            builder.HasMany(user => user.RefreshTokens)
                .WithOne(token => token.User)
                .HasForeignKey(token => token.UserId)
                .OnDelete(DeleteBehavior.Cascade); // If user is deleted, delete all refresh tokens
        }
    }
}

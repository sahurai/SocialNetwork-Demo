using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DataAccess.Entities.Post;
using SocialNetwork.Shared;

namespace SocialNetwork.DataAccess.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<PostEntity>
    {
        public void Configure(EntityTypeBuilder<PostEntity> builder)
        {
            // Primary key for the post
            builder.HasKey(post => post.Id);

            // Timestamp for when the post was edited
            builder.Property(post => post.EditedAt)
                .HasDefaultValue(null);

            // Content is required with a maximum length
            builder.Property(post => post.Content)
                .HasMaxLength(Constants.MaxPostContentLength)
                .IsRequired();

            // One-to-Many: Post -> Comments
            builder.HasMany(post => post.Comments)
                .WithOne(comment => comment.Post)
                .HasForeignKey(comment => comment.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Post -> Likes
            builder.HasMany(post => post.Likes)
                .WithOne(like => like.Post)
                .HasForeignKey(like => like.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

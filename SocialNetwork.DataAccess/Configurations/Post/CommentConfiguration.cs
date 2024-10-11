using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DataAccess.Entities.Post;
using SocialNetwork.Shared;

namespace SocialNetwork.DataAccess.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<CommentEntity>
    {
        public void Configure(EntityTypeBuilder<CommentEntity> builder)
        {
            // Primary key for the comment
            builder.HasKey(comment => comment.Id);

            // Content is required with a maximum length
            builder.Property(comment => comment.Content)
                .HasMaxLength(Constants.MaxCommentContentLength)
                .IsRequired();

            // Timestamp for when the comment was edited
            builder.Property(comment => comment.EditedAt)
                .HasDefaultValue(null);
        }
    }
}

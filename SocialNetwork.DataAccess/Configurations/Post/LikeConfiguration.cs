using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DataAccess.Entities.Post;

namespace SocialNetwork.DataAccess.Configurations
{
    public class LikeConfiguration : IEntityTypeConfiguration<LikeEntity>
    {
        public void Configure(EntityTypeBuilder<LikeEntity> builder)
        {
            // Primary key for the like
            builder.HasKey(like => like.Id);

            // Unique constraint to prevent duplicate likes
            builder.HasIndex(like => new { like.UserId, like.PostId, like.CommentId }).IsUnique();

            // Relationships
            builder.HasOne(like => like.User)
                .WithMany()
                .HasForeignKey(like => like.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(like => like.Post)
                .WithMany()
                .HasForeignKey(like => like.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(like => like.Comment)
                .WithMany()
                .HasForeignKey(like => like.CommentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DataAccess.Entities;
using SocialNetwork.DataAccess.Entities.User;

namespace SocialNetwork.DataAccess.Configurations
{
    public class UserBlockConfiguration : IEntityTypeConfiguration<UserBlockEntity>
    {
        public void Configure(EntityTypeBuilder<UserBlockEntity> builder)
        {
            // Primary key for the block
            builder.HasKey(block => block.Id);

            // Unique constraint to prevent duplicate blocks
            builder.HasIndex(b => new { b.BlockerId, b.BlockedId }).IsUnique();

            // Relationships
            builder.HasOne(block => block.Blocker)
                .WithMany(user => user.BlockedUsers)
                .HasForeignKey(block => block.BlockerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(block => block.Blocked)
                .WithMany()
                .HasForeignKey(block => block.BlockedId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

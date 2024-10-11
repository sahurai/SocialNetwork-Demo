using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DataAccess.Entities;

namespace SocialNetwork.DataAccess.Configurations
{
    public class FriendshipConfiguration : IEntityTypeConfiguration<FriendshipEntity>
    {
        public void Configure(EntityTypeBuilder<FriendshipEntity> builder)
        {
            // Primary key for the friendship
            builder.HasKey(friendship => friendship.Id);

            // Unique constraint to prevent duplicate friendships
            builder.HasIndex(f => new { f.User1Id, f.User2Id }).IsUnique();

            // Relationships
            builder.HasOne(friendship => friendship.User1)
                .WithMany()
                .HasForeignKey(friendship => friendship.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(friendship => friendship.User2)
                .WithMany()
                .HasForeignKey(friendship => friendship.User2Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(friendship => friendship.RequestedBy)
                .WithMany()
                .HasForeignKey(friendship => friendship.RequestedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DataAccess.Entities.Group;

namespace SocialNetwork.DataAccess.Configurations.Group
{
    public class GroupBlockConfiguration : IEntityTypeConfiguration<GroupBlockEntity>
    {
        public void Configure(EntityTypeBuilder<GroupBlockEntity> builder)
        {
            // Primary key for the group block
            builder.HasKey(block => block.Id);

            // The blocker user relationship (admin or manager blocking a user in the group)
            builder.HasOne(block => block.Blocker)
                .WithMany()
                .HasForeignKey(block => block.BlockerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade deletion of blocker

            // The blocked user relationship
            builder.HasOne(block => block.Blocked)
                .WithMany()
                .HasForeignKey(block => block.BlockedId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade deletion of blocked user

            // The group where the block applies
            builder.HasOne(block => block.Group)
                .WithMany()
                .HasForeignKey(block => block.GroupId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete if group is deleted

            // Index to prevent duplicate blocks in the same group
            builder.HasIndex(block => new { block.BlockerId, block.BlockedId, block.GroupId }).IsUnique();
        }
    }
}

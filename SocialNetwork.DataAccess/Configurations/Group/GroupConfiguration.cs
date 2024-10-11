using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DataAccess.Entities.Group;
using SocialNetwork.Shared;

namespace SocialNetwork.DataAccess.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<GroupEntity>
    {
        public void Configure(EntityTypeBuilder<GroupEntity> builder)
        {
            // Primary key for the group
            builder.HasKey(group => group.Id);

            // Group name is required with a maximum length constraint
            builder.Property(group => group.Name)
                .HasMaxLength(Constants.MaxGroupNameLength)
                .IsRequired();

            // Description of the group
            builder.Property(group => group.Description)
                .HasMaxLength(Constants.MaxGroupDescriptionLength);

            // One-to-Many: Group -> Members
            builder.HasMany(group => group.Members)
                .WithOne(member => member.Group)
                .HasForeignKey(member => member.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Group -> Posts
            builder.HasMany(group => group.Posts)
                .WithOne(post => post.Group)
                .HasForeignKey(post => post.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

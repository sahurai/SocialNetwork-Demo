using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DataAccess.Entities.Group;

namespace SocialNetwork.DataAccess.Configurations
{
    public class GroupUserRoleConfiguration : IEntityTypeConfiguration<GroupUserRoleEntity>
    {
        public void Configure(EntityTypeBuilder<GroupUserRoleEntity> builder)
        {
            // Primary key for the group user role
            builder.HasKey(role => role.Id);

            // Unique constraint to prevent duplicate role assignments
            builder.HasIndex(r => new { r.UserId, r.GroupId }).IsUnique();

            // Role is required
            builder.Property(role => role.Role)
                .IsRequired();

            // Relationships
            builder.HasOne(role => role.User)
                .WithMany()
                .HasForeignKey(role => role.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(role => role.Group)
                .WithMany(group => group.Members)
                .HasForeignKey(member => member.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

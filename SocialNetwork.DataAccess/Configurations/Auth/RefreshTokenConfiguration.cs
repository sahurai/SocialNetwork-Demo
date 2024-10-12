using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.DataAccess.Entities.Auth;

namespace SocialNetwork.DataAccess.Configurations.Auth
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
    {
        public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
        {
            builder.HasKey(token => token.Id);

            builder.Property(token => token.Token)
                .IsRequired();

            builder.Property(token => token.UserAgent)
                .IsRequired();

            builder.Property(token => token.IpAddress)
                .IsRequired();

            // One-to-Many: User -> RefreshTokens
            builder
                .HasOne(token => token.User)
                .WithMany(user => user.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade); // If user is deleted - delete all refresh tokens
        }
    }
}

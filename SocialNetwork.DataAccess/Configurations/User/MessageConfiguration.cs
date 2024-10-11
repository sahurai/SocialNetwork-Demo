using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DataAccess.Entities.User;
using SocialNetwork.Shared;

namespace SocialNetwork.DataAccess.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<MessageEntity>
    {
        public void Configure(EntityTypeBuilder<MessageEntity> builder)
        {
            // Primary key for the message
            builder.HasKey(message => message.Id);

            // Content is required with a maximum length
            builder.Property(message => message.Content)
                .HasMaxLength(Constants.MaxMessageContentLength)
                .IsRequired();

            // Timestamp for when the message was edited
            builder.Property(message => message.EditedAt)
                .HasDefaultValue(null);

            // Boolean to indicate if the message was read
            builder.Property(message => message.IsRead)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}

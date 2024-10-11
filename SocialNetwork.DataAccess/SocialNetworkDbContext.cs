using SocialNetwork.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.DataAccess.Entities.User;
using SocialNetwork.DataAccess.Entities.Group;
using SocialNetwork.DataAccess.Entities.Post;
using SocialNetwork.DataAccess.Configurations;
using SocialNetwork.DataAccess.Configurations.Group;

namespace SocialNetwork.DataAccess
{
    public class SocialNetworkDbContext : DbContext
    {
        public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options) : base(options) { }

        // DbSets for the main entities
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<PostEntity> Posts { get; set; }
        public DbSet<CommentEntity> Comments { get; set; }
        public DbSet<LikeEntity> Likes { get; set; }
        public DbSet<MessageEntity> Messages { get; set; }
        public DbSet<FriendshipEntity> Friendships { get; set; }
        public DbSet<UserBlockEntity> UserBlocks { get; set; }
        public DbSet<GroupEntity> Groups { get; set; }
        public DbSet<GroupUserRoleEntity> GroupUserRoles { get; set; }
        public DbSet<GroupBlockEntity> GroupBlocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Applying configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new LikeConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new FriendshipConfiguration());
            modelBuilder.ApplyConfiguration(new UserBlockConfiguration());
            modelBuilder.ApplyConfiguration(new GroupConfiguration());
            modelBuilder.ApplyConfiguration(new GroupUserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new GroupBlockConfiguration());
        }
    }
}

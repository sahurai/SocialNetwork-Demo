using Microsoft.EntityFrameworkCore;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Entities.User;

namespace SocialNetwork.DataAccess.Repositories
{
    public class UserBlockRepository : IUserBlockRepository
    {
        private readonly SocialNetworkDbContext _context;

        public UserBlockRepository(SocialNetworkDbContext context)
        {
            _context = context;
        }

        // Retrieve user blocks with optional filtering
        public async Task<List<UserBlock>> GetAsync(Guid? userBlockId = null, Guid? blockerId = null, Guid? blockedId = null)
        {
            IQueryable<UserBlockEntity> query = _context.UserBlocks.AsNoTracking();

            if (userBlockId.HasValue)
            {
                query = query.Where(block => block.Id == userBlockId.Value);
            }

            if (blockerId.HasValue)
            {
                query = query.Where(block => block.BlockerId == blockerId.Value);
            }

            if (blockedId.HasValue)
            {
                query = query.Where(block => block.BlockedId == blockedId.Value);
            }

            List<UserBlockEntity> blockEntities = await query.ToListAsync();

            List<UserBlock> blocks = blockEntities.Select(MapToModel).ToList();

            return blocks;
        }

        // Create a new user block
        public async Task<UserBlock> CreateAsync(UserBlock userBlock)
        {
            UserBlockEntity blockEntity = new UserBlockEntity
            {
                BlockerId = userBlock.BlockerId,
                BlockedId = userBlock.BlockedId,
            };

            await _context.UserBlocks.AddAsync(blockEntity);
            await _context.SaveChangesAsync();

            return MapToModel(blockEntity);
        }

        // Delete a user block by Id
        public async Task<Guid> DeleteAsync(Guid id)
        {
            await _context.UserBlocks
                .Where(userBlock => userBlock.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }

        // Map UserBlockEntity to UserBlock model
        private UserBlock MapToModel(UserBlockEntity entity)
        {
            var (userBlock, error) = UserBlock.CreateFromDb(
                entity.Id,
                entity.BlockerId,
                entity.BlockedId,
                entity.CreatedAt,
                entity.UpdatedAt);

            if (!string.IsNullOrEmpty(error)) throw new Exception($"Error during mapping: {error}");

            return userBlock!;
        }
    }
}

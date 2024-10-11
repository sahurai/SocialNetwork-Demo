using Microsoft.EntityFrameworkCore;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Entities.Group;

namespace SocialNetwork.DataAccess.Repositories
{
    public class GroupBlockRepository : IGroupBlockRepository
    {
        private readonly SocialNetworkDbContext _context;

        public GroupBlockRepository(SocialNetworkDbContext context)
        {
            _context = context;
        }

        // Retrieve group blocks with optional filtering
        public async Task<List<GroupBlock>> GetAsync(Guid? groupBlockId = null, Guid? groupId = null, Guid? blockerId = null, Guid? blockedId = null)
        {
            IQueryable<GroupBlockEntity> query = _context.Set<GroupBlockEntity>().AsNoTracking();

            if (groupBlockId.HasValue)
            {
                query = query.Where(block => block.Id == groupBlockId.Value);
            }

            if (groupId.HasValue)
            {
                query = query.Where(block => block.GroupId == groupId.Value);
            }

            if (blockerId.HasValue)
            {
                query = query.Where(block => block.BlockerId == blockerId.Value);
            }

            if (blockedId.HasValue)
            {
                query = query.Where(block => block.BlockedId == blockedId.Value);
            }

            List<GroupBlockEntity> blockEntities = await query.ToListAsync();

            List<GroupBlock> blocks = blockEntities.Select(MapToModel).ToList();

            return blocks;
        }

        // Create a new group block
        public async Task<GroupBlock> CreateAsync(GroupBlock groupBlock)
        {
            GroupBlockEntity blockEntity = new GroupBlockEntity
            {
                BlockerId = groupBlock.BlockerId,
                BlockedId = groupBlock.BlockedId,
                GroupId = groupBlock.GroupId
            };

            await _context.Set<GroupBlockEntity>().AddAsync(blockEntity);
            await _context.SaveChangesAsync();

            return MapToModel(blockEntity);
        }

        // Delete a group block by Id
        public async Task<Guid> DeleteAsync(Guid id)
        {
            await _context.GroupBlocks
                .Where(groupBlock => groupBlock.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }

        // Map GroupBlockEntity to GroupBlock model
        private GroupBlock MapToModel(GroupBlockEntity entity)
        {
            var (groupBlock, error) = GroupBlock.CreateFromDb(
                entity.Id,
                entity.BlockerId,
                entity.BlockedId,
                entity.GroupId,
                entity.CreatedAt,
                entity.UpdatedAt);

            if (!string.IsNullOrEmpty(error)) throw new Exception($"Error during mapping: {error}");

            return groupBlock!;
        }
    }
}

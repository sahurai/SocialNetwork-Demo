using Microsoft.EntityFrameworkCore;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Entities.Group;

namespace SocialNetwork.DataAccess.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly SocialNetworkDbContext _context;

        public GroupRepository(SocialNetworkDbContext context)
        {
            _context = context;
        }

        // Retrieve groups with optional filtering
        public async Task<List<Group>> GetAsync(Guid? groupId = null, Guid? creatorId = null, string? name = null, string? description = null)
        {
            IQueryable<GroupEntity> query = _context.Groups.AsNoTracking();

            if (groupId.HasValue)
            {
                query = query.Where(group => group.Id == groupId.Value);
            }

            if (creatorId.HasValue)
            {
                query = query.Where(group => group.CreatorId == creatorId.Value);
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(group => group.Name.ToLower().Contains(name.ToLower()));
            }

            if (!string.IsNullOrEmpty(description))
            {
                query = query.Where(group => group.Description != null && group.Description.ToLower().Contains(description.ToLower()));
            }

            query = query.OrderByDescending(group => group.CreatedAt);

            List<GroupEntity> groupEntities = await query.ToListAsync();

            List<Group> groups = groupEntities.Select(MapToModel).ToList();

            return groups;
        }

        // Create a new group
        public async Task<Group> CreateAsync(Group group)
        {
            GroupEntity groupEntity = new GroupEntity
            {
                CreatorId = group.CreatorId,
                Name = group.Name,
                Description = group.Description
            };

            await _context.Groups.AddAsync(groupEntity);
            await _context.SaveChangesAsync();

            return MapToModel(groupEntity);
        }

        // Update an existing group
        public async Task<Group> UpdateAsync(Guid id, Group updatedGroup)
        {
            await _context.Groups
                .Where(group => group.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(group => group.Name, updatedGroup.Name)
                    .SetProperty(group => group.Description, updatedGroup.Description)
                    .SetProperty(group => group.CreatorId, updatedGroup.CreatorId)
                    .SetProperty(group => group.UpdatedAt, updatedGroup.UpdatedAt));

            return updatedGroup;
        }

        // Delete a group by Id
        public async Task<Guid> DeleteAsync(Guid id)
        {
            await _context.Groups
                .Where(group => group.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }

        // Map GroupEntity to Group model
        private Group MapToModel(GroupEntity entity)
        {
            var (group, error) = Group.CreateFromDb(
                entity.Id,
                entity.CreatorId,
                entity.Name,
                entity.Description,
                entity.CreatedAt,
                entity.UpdatedAt);

            if (!string.IsNullOrEmpty(error)) throw new Exception($"Error during mapping: {error}");

            return group!;
        }
    }
}

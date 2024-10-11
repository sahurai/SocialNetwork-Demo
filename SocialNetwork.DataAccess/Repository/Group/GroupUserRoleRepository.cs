using Microsoft.EntityFrameworkCore;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Entities.Group;


namespace SocialNetwork.DataAccess.Repositories
{
    public class GroupUserRoleRepository : IGroupUserRoleRepository
    {
        private readonly SocialNetworkDbContext _context;

        public GroupUserRoleRepository(SocialNetworkDbContext context)
        {
            _context = context;
        }

        // Retrieve group user roles with optional filtering
        public async Task<List<GroupUserRole>> GetAsync(Guid? groupUserRoleId = null, Guid? groupId = null, Guid? userId = null)
        {
            IQueryable<GroupUserRoleEntity> query = _context.GroupUserRoles.AsNoTracking();

            if (groupUserRoleId.HasValue)
            {
                query = query.Where(role => role.Id == groupUserRoleId.Value);
            }

            if (groupId.HasValue)
            {
                query = query.Where(role => role.GroupId == groupId.Value);
            }

            if (userId.HasValue)
            {
                query = query.Where(role => role.UserId == userId.Value);
            }

            query = query.OrderByDescending(role => role.CreatedAt);

            List<GroupUserRoleEntity> roleEntities = await query.ToListAsync();

            List<GroupUserRole> roles = roleEntities.Select(MapToModel).ToList();

            return roles;
        }

        // Create a new group user role
        public async Task<GroupUserRole> CreateAsync(GroupUserRole groupUserRole)
        {
            GroupUserRoleEntity roleEntity = new GroupUserRoleEntity
            {
                UserId = groupUserRole.UserId,
                GroupId = groupUserRole.GroupId,
                Role = groupUserRole.Role
            };

            await _context.GroupUserRoles.AddAsync(roleEntity);
            await _context.SaveChangesAsync();

            return MapToModel(roleEntity);
        }

        // Update an existing group user role
        public async Task<GroupUserRole> UpdateAsync(Guid id, GroupUserRole updatedGroupUserRole)
        {
            await _context.GroupUserRoles
               .Where(groupUserRole => groupUserRole.Id == id)
               .ExecuteUpdateAsync(s => s
                   .SetProperty(groupUserRole => groupUserRole.Role, updatedGroupUserRole.Role)
                   .SetProperty(groupUserRole => groupUserRole.UpdatedAt, updatedGroupUserRole.UpdatedAt));

            return updatedGroupUserRole;
        }

        // Delete a group user role by Id
        public async Task<Guid> DeleteAsync(Guid id)
        {
            await _context.GroupUserRoles
                .Where(groupUserRole => groupUserRole.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }

        // Map GroupUserRoleEntity to GroupUserRole model
        private GroupUserRole MapToModel(GroupUserRoleEntity entity)
        {
            var (role, error) = GroupUserRole.CreateFromDb(
                entity.Id,
                entity.UserId,
                entity.GroupId,
                entity.Role,
                entity.CreatedAt,
                entity.UpdatedAt);

            if (!string.IsNullOrEmpty(error)) throw new Exception($"Error during mapping: {error}");

            return role!;
        }
    }
}

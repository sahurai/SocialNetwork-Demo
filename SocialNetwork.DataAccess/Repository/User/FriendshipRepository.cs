using Microsoft.EntityFrameworkCore;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Entities;

namespace SocialNetwork.DataAccess.Repositories
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly SocialNetworkDbContext _context;

        public FriendshipRepository(SocialNetworkDbContext context)
        {
            _context = context;
        }

        // Retrieve friendships involving a specific user
        public async Task<List<Friendship>> GetAsync(Guid? friendshipId = null, Guid? userId = null)
        {
            IQueryable<FriendshipEntity> query = _context.Friendships.AsNoTracking();

            if (friendshipId.HasValue)
            {
                query = query.Where(friendship => friendship.Id == friendshipId);
            }

            if (userId.HasValue)
            {
                query = query.Where(friendship => friendship.User1Id == userId.Value || friendship.User2Id == userId.Value);
            }

            query = query.OrderByDescending(friendship => friendship.CreatedAt);

            List<FriendshipEntity> friendshipEntities = await query.ToListAsync();

            List<Friendship> friendships = friendshipEntities.Select(MapToModel).ToList();

            return friendships;
        }

        // Get friendship between two users
        public async Task<Friendship?> GetFriendshipBetweenUsersAsync(Guid userId1, Guid userId2)
        {
            var friendshipEntity = await _context.Friendships.AsNoTracking()
                .FirstOrDefaultAsync(f =>
                    (f.User1Id == userId1 && f.User2Id == userId2) ||
                    (f.User1Id == userId2 && f.User2Id == userId1));

            return friendshipEntity != null ? MapToModel(friendshipEntity) : null;
        }

        // Create a new friendship
        public async Task<Friendship> CreateAsync(Friendship friendship)
        {
            FriendshipEntity friendshipEntity = new FriendshipEntity
            {
                User1Id = friendship.User1Id,
                User2Id = friendship.User2Id,
                RequestedById = friendship.RequestedById,
                AcceptedAt = friendship.AcceptedAt
            };

            await _context.Friendships.AddAsync(friendshipEntity);
            await _context.SaveChangesAsync();

            return MapToModel(friendshipEntity);
        }

        // Update an existing friendship
        public async Task<Friendship> UpdateAsync(Guid id, Friendship updatedFriendship)
        {
            await _context.Friendships
               .Where(friendship => friendship.Id == id)
               .ExecuteUpdateAsync(s => s
                   .SetProperty(friendship => friendship.AcceptedAt, updatedFriendship.AcceptedAt)
                   .SetProperty(friendship => friendship.UpdatedAt, updatedFriendship.UpdatedAt));

            return updatedFriendship;
        }

        // Delete a friendship by Id
        public async Task<Guid> DeleteAsync(Guid id)
        {
            await _context.Friendships
                .Where(friendship => friendship.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }

        // Map FriendshipEntity to Friendship model
        private Friendship MapToModel(FriendshipEntity entity)
        {
            var (friendship, error) = Friendship.CreateFromDb(
                entity.Id,
                entity.User1Id,
                entity.User2Id,
                entity.RequestedById,
                entity.AcceptedAt,
                entity.CreatedAt,
                entity.UpdatedAt);

            if (!string.IsNullOrEmpty(error)) throw new Exception($"Error during mapping: {error}");

            return friendship!;
        }
    }
}

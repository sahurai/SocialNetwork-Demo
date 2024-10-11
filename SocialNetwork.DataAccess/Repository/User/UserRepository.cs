using Microsoft.EntityFrameworkCore;
using SocialNetwork.Core.Interfaces.Repositories.UserRepository;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Entities;

namespace SocialNetwork.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SocialNetworkDbContext _context;

        public UserRepository(SocialNetworkDbContext context)
        {
            _context = context;
        }

        // Retrieve users with optional filtering
        public async Task<List<User>> GetAsync(Guid? userId = null, string? username = null, string? email = null)
        {
            IQueryable<UserEntity> query = _context.Users.AsNoTracking();

            if (userId.HasValue)
            {
                query = query.Where(user => user.Id == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(username))
            {
                query = query.Where(user => user.Username == username);
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(user => user.Email == email);
            }

            query = query.OrderByDescending(user => user.CreatedAt);

            List<UserEntity> userEntities = await query.ToListAsync();

            List<User> users = userEntities.Select(MapToModel).ToList();

            return users;
        }

        // Create a new user
        public async Task<User> CreateAsync(User user)
        {
            UserEntity userEntity = new UserEntity
            {
                Username = user.Username,
                Email = user.Email,
                PasswordHash = user.PasswordHash
            };

            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();

            return MapToModel(userEntity);
        }

        // Update an existing user
        public async Task<User> UpdateAsync(Guid id, User updatedUser)
        {
            await _context.Users
                .Where(user => user.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(user => user.Username, updatedUser.Username)
                    .SetProperty(user => user.Email, updatedUser.Email)
                    .SetProperty(user => user.PasswordHash, updatedUser.PasswordHash)
                    .SetProperty(user => user.UpdatedAt, updatedUser.UpdatedAt));

            return updatedUser;
        }

        // Delete a user by Id
        public async Task<Guid> DeleteAsync(Guid id)
        {
            await _context.Users
                .Where(user => user.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }

        // Map UserEntity to User model
        private User MapToModel(UserEntity entity)
        {
            var (user, error) = User.CreateFromDb(
                entity.Id,
                entity.Username,
                entity.Email,
                entity.PasswordHash,
                entity.CreatedAt,
                entity.UpdatedAt);

            if (!string.IsNullOrEmpty(error)) throw new Exception($"Error during mapping: {error}");

            return user!;
        }
    }
}

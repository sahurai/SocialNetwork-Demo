using SocialNetwork.Core.Enums;
using SocialNetwork.Core.Models;

namespace SocialNetwork.Core.Interfaces.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user);
        Task<Guid> DeleteAsync(Guid id);
        Task<List<User>> GetAsync(Guid? userId = null, string? username = null, string? email = null, UserRole? role = null);
        Task<User> UpdateAsync(Guid id, User updatedUser);
    }
}
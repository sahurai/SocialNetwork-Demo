using Microsoft.Extensions.Logging;
using SocialNetwork.Core.Interfaces.Repositories.UserRepository;
using SocialNetwork.Core.Interfaces.Services.UserService;
using SocialNetwork.Core.Models;

namespace SocialNetwork.ApplicationLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        // Retrieve users with optional filtering
        public async Task<(List<User> Users, string Error)> GetUsersAsync(Guid? userId = null, string? username = null, string? email = null)
        {
            try
            {
                var users = await _userRepository.GetAsync(userId, username, email);
                return (users, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users.");
                return (new List<User>(), "An error occurred while retrieving users.");
            }
        }

        // Create a new user
        public async Task<(User? User, string Error)> CreateUserAsync(string username, string email, string password)
        {
            try
            {
                // Check if email already exists
                var existingUsers = await _userRepository.GetAsync(email: email);
                if (existingUsers.Any()) return (null, "An account with this email already exists.");

                // Hash the password
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                // Create the user model
                var (user, createError) = User.Create(username, email, passwordHash);
                if (user == null) return (null, createError);

                // Save to the database
                var createdUser = await _userRepository.CreateAsync(user);
                return (createdUser, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a user.");
                return (null, "An error occurred while creating the user.");
            }
        }

        // Update an existing user
        public async Task<(User? User, string Error)> UpdateUserAsync(Guid id, string? username, string? email, string? password)
        {
            try
            {
                // Retrieve the user
                var users = await _userRepository.GetAsync(userId: id);
                var user = users.FirstOrDefault();
                if (user == null) return (null, "User not found.");

                // Check if email is being updated and if it already exists
                if (!string.IsNullOrEmpty(email) && email != user.Email)
                {
                    var existingUsers = await _userRepository.GetAsync(email: email);
                    if (existingUsers.Any(u => u.Id != id))
                    {
                        return (null, "An account with this email already exists.");
                    }
                }

                // Update fields
                string updatedUsername = username ?? user.Username;
                string updatedEmail = email ?? user.Email;
                string updatedPasswordHash = password != null ? BCrypt.Net.BCrypt.HashPassword(password) : user.PasswordHash;

                // Create updated user model
                var (updatedUser, createError) = User.CreateFromDb(
                    user.Id,
                    updatedUsername,
                    updatedEmail,
                    updatedPasswordHash,
                    user.CreatedAt,
                    DateTime.UtcNow);
                if (updatedUser == null) return (null, createError);

                // Update in the database
                var result = await _userRepository.UpdateAsync(id, updatedUser);
                return (result, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user.");
                return (null, "An error occurred while updating the user.");
            }
        }

        // Delete a user
        public async Task<(Guid Id, string Error)> DeleteUserAsync(Guid id, Guid requestingUserId)
        {
            try
            {
                // Retrieve the user
                var users = await _userRepository.GetAsync(userId: id);
                var user = users.FirstOrDefault();
                if (user == null) return (Guid.Empty, "User not found.");

                // Check if the requesting user is the same as the user to be deleted
                if (user.Id != requestingUserId) return (Guid.Empty, "You can only delete your own account.");

                // Delete from the database
                var deletedId = await _userRepository.DeleteAsync(id);
                return (deletedId, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the user.");
                return (Guid.Empty, "An error occurred while deleting the user.");
            }
        }
    }
}

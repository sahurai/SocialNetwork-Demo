using Microsoft.Extensions.Logging;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Repositories;

namespace SocialNetwork.ApplicationLogic.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ILogger<FriendshipService> _logger;

        public FriendshipService(IFriendshipRepository friendshipRepository, ILogger<FriendshipService> logger)
        {
            _friendshipRepository = friendshipRepository;
            _logger = logger;
        }

        // Retrieve friendships for a user
        public async Task<(List<Friendship> Friendships, string Error)> GetFriendshipsAsync(Guid userId)
        {
            try
            {
                var friendships = await _friendshipRepository.GetAsync(userId: userId);
                return (friendships, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving friendships for user {userId}.");
                return (new List<Friendship>(), "An error occurred while retrieving friendships.");
            }
        }

        // Create a new friendship request
        public async Task<(Friendship? Friendship, string Error)> CreateFriendshipAsync(Guid requestingUserId, Guid user2Id)
        {
            try
            {
                // Ensure users are not the same
                if (requestingUserId == user2Id) return (null, "You cannot befriend yourself.");

                // Check if friendship already exists
                var existingFriendship = await _friendshipRepository.GetFriendshipBetweenUsersAsync(requestingUserId, user2Id);
                if (existingFriendship != null) return (null, "Friendship already exists.");

                // Create the friendship model
                var (friendship, createError) = Friendship.Create(requestingUserId, user2Id);
                if (friendship == null) return (null, createError);

                // Save to the database
                var createdFriendship = await _friendshipRepository.CreateAsync(friendship);
                return (createdFriendship, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while creating a friendship between {requestingUserId} and {user2Id}.");
                return (null, "An error occurred while creating the friendship.");
            }
        }

        // Accept a friendship request
        public async Task<(Friendship? Friendship, string Error)> AcceptFriendshipAsync(Guid friendshipId, Guid userId)
        {
            try
            {
                // Retrieve the friendship by Id
                var friendships = await _friendshipRepository.GetAsync(friendshipId: friendshipId);
                var friendship = friendships.FirstOrDefault();
                if (friendship == null) return (null, "Friendship not found.");

                // Ensure only the recipient can accept the friendship
                if (friendship.User2Id != userId)
                    return (null, "Only the recipient can accept the friendship request.");

                // Accept the friendship
                friendship.Accept();

                // Update in the database
                var result = await _friendshipRepository.UpdateAsync(friendshipId, friendship);
                return (result, string.Empty);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, $"Friendship {friendshipId} has already been accepted.");
                return (null, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while accepting the friendship {friendshipId} by user {userId}.");
                return (null, "An error occurred while accepting the friendship.");
            }
        }

        // Delete a friendship
        public async Task<(Guid Id, string Error)> DeleteFriendshipAsync(Guid friendshipId, Guid requestingUserId)
        {
            try
            {
                // Retrieve the friendship
                var friendships = await _friendshipRepository.GetAsync(friendshipId: friendshipId);
                var friendship = friendships.FirstOrDefault();
                if (friendship == null) return (Guid.Empty, "Friendship not found.");

                // Ensure the user is part of the friendship
                if (friendship.User1Id != requestingUserId && friendship.User2Id != requestingUserId)
                    return (Guid.Empty, "You are not part of this friendship.");

                // Delete from the database
                var deletedId = await _friendshipRepository.DeleteAsync(friendshipId);
                return (deletedId, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the friendship.");
                return (Guid.Empty, "An error occurred while deleting the friendship.");
            }
        }
    }
}

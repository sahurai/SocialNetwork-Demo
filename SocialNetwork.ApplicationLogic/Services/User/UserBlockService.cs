using Microsoft.Extensions.Logging;
using SocialNetwork.Core.Models;
using SocialNetwork.DataAccess.Repositories;

namespace SocialNetwork.ApplicationLogic.Services
{
    public class UserBlockService : IUserBlockService
    {
        private readonly IUserBlockRepository _blockRepository;
        private readonly ILogger<UserBlockService> _logger;

        public UserBlockService(IUserBlockRepository blockRepository, ILogger<UserBlockService> logger)
        {
            _blockRepository = blockRepository;
            _logger = logger;
        }

        // Retrieve user blocks with optional filtering
        public async Task<(List<UserBlock> Blocks, string Error)> GetUserBlocksAsync(Guid? blockerId = null, Guid? blockedId = null)
        {
            try
            {
                var blocks = await _blockRepository.GetAsync(blockerId, blockedId);
                return (blocks, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user blocks.");
                return (new List<UserBlock>(), "An error occurred while retrieving user blocks.");
            }
        }

        // Create a new user block
        public async Task<(UserBlock? Block, string Error)> CreateUserBlockAsync(Guid requestingUserId, Guid blockedId)
        {
            try
            {
                // Create the block model
                var (userBlock, createError) = UserBlock.Create(requestingUserId, blockedId);
                if (userBlock == null) return (null, createError);

                // Save to the database
                var createdBlock = await _blockRepository.CreateAsync(userBlock);
                return (createdBlock, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a user block.");
                return (null, "An error occurred while creating the user block.");
            }
        }

        // Delete a user block
        public async Task<(Guid Id, string Error)> DeleteUserBlockAsync(Guid userBlockId, Guid requestingUserId)
        {
            try
            {
                // Retrieve the block
                var blocks = await _blockRepository.GetAsync(userBlockId: userBlockId);
                var userBlock = blocks.FirstOrDefault();
                if (userBlock == null) return (Guid.Empty, "User block not found.");

                // Ensure that this is the creator of block
                if (userBlock.BlockerId != requestingUserId) return (Guid.Empty, "You can only delete your blocked users.");

                // Delete from the database
                var deletedId = await _blockRepository.DeleteAsync(userBlockId);
                return (deletedId, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the user block.");
                return (Guid.Empty, "An error occurred while deleting the user block.");
            }
        }
    }
}

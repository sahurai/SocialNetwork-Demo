using System.Security.Claims;

namespace SocialNetwork.Shared
{
    public static class Utilities
    {
        // Get user id
        public static (Guid? UserId, string Error) ExtractUserIdFromClaimsPrincipal(ClaimsPrincipal user)
        {
            try
            {
                // Getting id
                var userIdClaim = user.FindFirst("userId");
                if (userIdClaim == null) return (null, "UserId claim is missing in the user claims.");

                // Converting to Guid
                if (Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return (userId, string.Empty);
                }

                return (null, "Invalid UserId in the claims.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while extracting userId from the claims: {ex.Message}");
                return (null, "An error occurred while extracting userId from the claims.");
            }
        }
    }
}

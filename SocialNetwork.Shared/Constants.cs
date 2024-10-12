namespace SocialNetwork.Shared
{
    public static class Constants
    {
        // Auth
        public const int RefreshTokenExpiresInDays = 7;
        public const int AccessTokenExpiresInMinutes = 15;
        public const int RefreshRefreshTokenInDays = 1;

        // User
        public const int MaxUsernameLength = 100;
        public const int MaxEmailLength = 255;

        // Message
        public const int MaxMessageContentLength = 500;

        // Group
        public const int MaxGroupNameLength = 255;
        public const int MaxGroupDescriptionLength = 500;

        // Post
        public const int MaxPostContentLength = 5000;

        // Comment
        public const int MaxCommentContentLength = 500;
    }
}

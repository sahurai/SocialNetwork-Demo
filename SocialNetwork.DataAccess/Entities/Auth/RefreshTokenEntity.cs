namespace SocialNetwork.DataAccess.Entities.Auth
{
    public class RefreshTokenEntity : BaseEntity
    {
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string UserAgent { get; set; }
        public string IpAddress { get; set; }

        public UserEntity User { get; set; }
    }
}

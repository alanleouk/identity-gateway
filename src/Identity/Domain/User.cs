// ReSharper disable once CheckNamespace
namespace Identity
{
    public class User
    {
        public Guid Id { get; set; }

        public string? Username { get; set; }
        public string? Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }

        public DateTime? LockoutEndUtc { get; set; }
        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        // Linked Entities
        
        public IList<UserClaim>? Claims { get; set; }
        public IList<UserExternalLogin>? ExternalLogins { get; set; }
        public IList<UserToken>? UserTokens { get; set; }
        public IList<UserOneTimePassword>? OneTimePasswords { get; set; }
    }
}

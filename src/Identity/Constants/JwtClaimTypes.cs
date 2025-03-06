
namespace Identity.Constants
{
    // See: https://github.com/IdentityModel/IdentityModel/blob/main/src/JwtClaimTypes.cs

    public static class JwtClaimTypes
    {
        public const string AccessTokenHash = "at_hash";
        public const string AuthenticationMethod = "amr";
        public const string Audience = "aud";
        public const string ClientId = "client_id";
        public const string Email = "email";
        public const string Nonce = "nonce";
        public const string Role = "role";
        public const string Scope = "scope";
    }
}

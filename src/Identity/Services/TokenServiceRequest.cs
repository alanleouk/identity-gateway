
namespace Identity.Services
{
    public class TokenServiceRequest
    {
        public IList<string>? Audience { get; set; }
        public string? Subject { get; set; }
        public string? Email { get; set; }
        public string? ClientId { get; set; }
        public IList<string>? Roles { get; set; }
        public IList<string>? Scopes { get; set; }
        public string? Nonce { get; set; }
        public string? PublicKey { get; set; }
    }
}

// ReSharper disable once CheckNamespace
namespace Identity
{
    public class UserClaim
    {
        public Guid Id { get; set; }
        public User? User { get; set; }
        
        public required string Type { get; set; }
        public required string Value { get; set; }
    }
}

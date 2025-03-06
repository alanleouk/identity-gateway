// ReSharper disable once CheckNamespace
namespace Identity
{
    public class UserOneTimePassword
    {
        public Guid Id { get; set; }

        public required User User { get; set; }
        
        public required string Key { get; set; }
        
    }
}
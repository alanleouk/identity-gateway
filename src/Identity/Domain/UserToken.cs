namespace Identity
{
    public class UserToken
    {
        public Guid Id { get; set; }

        public required User User { get; set; }
        
        public required string Name { get; set; }
        
        public required string Key { get; set; }

        public int? RemainingUses { get; set; }
        public DateTime? ExpiryDateUtc { get; set; }
        public bool IsEnabled { get; set; }
    }
}
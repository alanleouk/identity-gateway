namespace Identity
{
    [Serializable]
    public class UserExternalLogin
    {
        public Guid Id { get; set; }
        public User? User { get; set; }
        
        public required string Provider { get; set; }
        
        public required string Name { get; set; }
        
        public required string Key { get; set; }
    }
}

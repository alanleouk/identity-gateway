namespace Identity.Models.FindModels
{
    public class ByUsername
    {
        public string Username { get; }

        public ByUsername(string username)
        {
            Username = username;
        }
    }
}
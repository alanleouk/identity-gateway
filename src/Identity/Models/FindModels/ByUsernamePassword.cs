namespace Identity.Models.FindModels
{
    public class ByUsernamePassword
    {
        public string Username { get; }
        public string Password { get; }

        public ByUsernamePassword(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}

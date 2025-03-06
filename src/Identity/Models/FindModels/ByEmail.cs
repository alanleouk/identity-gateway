namespace Identity.Models.FindModels
{
    public class ByEmail
    {
        public string Email { get; init; }

        public ByEmail(string email)
        {
            Email = email;
        }
    }
}
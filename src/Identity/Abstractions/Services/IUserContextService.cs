namespace Identity.Services
{
    public interface IUserContextService
    {
        Guid? UserId { get; }
        string? BearerToken { get; }
        string? PublicKey { get; }

        bool IsAuthenticated();
        bool IsInRole(string role);
    }
}
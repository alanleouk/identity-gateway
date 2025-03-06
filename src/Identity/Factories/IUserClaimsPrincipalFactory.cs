using System.Security.Claims;

namespace Identity.Factories
{
    public interface IUserClaimsPrincipalFactory
    {
        Task<ClaimsPrincipal> CreateAsync(User user, string authenticationType);
    }
}

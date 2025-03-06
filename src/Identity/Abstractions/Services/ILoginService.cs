using System.Security.Claims;
using Identity.Models.IdentityModels;
using Microsoft.AspNetCore.Authentication;

namespace Identity.Services
{
    public interface ILoginService
    {
        Task<MyIdentityResult> ExternalLoginSignInAsync(MyExternalLoginInfo info, bool isPersistent);
        Task<MyIdentityResult> LoginAsync(User user, bool isPersistent, string? loginProvider);
        
        Task LogoutAsync();
        Task<ClaimsPrincipal> CreateUserPrincipalAsync(User user, string loginProvider);

        AuthenticationProperties ConfigureExternalAuthenticationProperties(
            string provider, string redirectUrl, Guid? userId = null);

        Task<MyExternalLoginInfo?> GetUserExternalLoginInfoAsync(Guid? expectedXsrf = null);

        Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();

        bool IsLoggedIn(ClaimsPrincipal principal);
        
        Task<MyIdentityResult> SetAuthenticationClaim(User user, string type);
    }
}
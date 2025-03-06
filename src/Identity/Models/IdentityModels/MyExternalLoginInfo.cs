using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Identity.Models.IdentityModels
{
    public class MyExternalLoginInfo : MyUserLoginInfo
    {
        public ClaimsPrincipal Principal { get; set; }
        public IEnumerable<AuthenticationToken> AuthenticationTokens { get; set; }
        public AuthenticationProperties AuthenticationProperties { get; set; }
        
        public MyExternalLoginInfo(ClaimsPrincipal principal, string loginProvider, string providerKey,
            string displayName) : base(loginProvider, providerKey, displayName)
        {
            Principal = principal;
        }
    }
}

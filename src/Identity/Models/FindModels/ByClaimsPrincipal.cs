using System.Security.Claims;

namespace Identity.Models.FindModels
{
    public class ByClaimsPrincipal
    {
        public ClaimsPrincipal Principal { get; }

        public ByClaimsPrincipal(ClaimsPrincipal principal)
        {
            Principal = principal;
        }
    }
}

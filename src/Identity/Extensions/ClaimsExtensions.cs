using System.Security.Claims;

namespace Identity.Extensions;

public static class ClaimsExtensions
{
    public static string AuthenticationType(this IEnumerable<Claim> claims, string fallback)
    {
        return claims.FirstOrDefault(item => item.Type == ClaimTypes.AuthenticationMethod)?.Value ?? fallback;
    }
}

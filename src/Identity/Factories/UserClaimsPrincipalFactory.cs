using System.Security.Claims;

namespace Identity.Factories
{
    public class UserClaimsPrincipalFactory : IUserClaimsPrincipalFactory
    {
        public virtual async Task<ClaimsPrincipal> CreateAsync(User user, string authenticationType)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var id = await GenerateClaimsAsync(user, authenticationType);
            return new ClaimsPrincipal(id);
        }

        protected virtual async Task<ClaimsIdentity> GenerateClaimsAsync(User user, string authenticationType)
        {
            var userId = user.Id.ToString().ToLower();
            var username = user.Username;
            var id = new ClaimsIdentity(authenticationType,
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            id.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
            if (!string.IsNullOrWhiteSpace(username))
            {
                id.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, username));
            }

            if (user.SecurityStamp != null)
            {
                id.AddClaim(new Claim("AspNet.Identity.SecurityStamp", user.SecurityStamp)); // TODO: Use claim const
            }

            /*
            if (UserManager.SupportsUserClaim)
            {
                id.AddClaims(await UserManager.GetClaimsAsync(user));
            }
            */
            return id;
        }
    }
}

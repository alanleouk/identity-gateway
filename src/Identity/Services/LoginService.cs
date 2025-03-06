using System.Security.Claims;
using Identity.Factories;
using Identity.Models.IdentityModels;
using Identity.Models.FindModels;
using Identity.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

// See: https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Core/src/SignInManager.cs

namespace Identity.Services
{
    public class LoginService : ILoginService
    {
        private const string LoginProviderKey = "LoginProvider";
        private const string XsrfKey = "XsrfId";

        private readonly IdentityDbContext _db;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserClaimsPrincipalFactory _claimsFactory;
        private readonly IAuthenticationSchemeProvider _schemes;

        public LoginService(
            IdentityDbContext db,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            IUserClaimsPrincipalFactory claimsFactory,
            IAuthenticationSchemeProvider schemes)
        {
            _db = db;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _claimsFactory = claimsFactory;
            _schemes = schemes;
        }

        public virtual async Task<MyIdentityResult> ExternalLoginSignInAsync(MyExternalLoginInfo info, bool isPersistent)
        {
            var user = await _userService.FindAsync(new ByExternalLogin(info.LoginProvider, info.ProviderKey));
            if (user == null)
            {
                return MyIdentityResult.FailedResult();
            }
            
            return await LoginAsync(user, isPersistent, info.LoginProvider);
        }
        
        public virtual async Task<MyIdentityResult> LoginAsync(
            User user, 
            bool isPersistent,
            string? loginProvider)
        {
            // Cleanup external cookie
            if (loginProvider != null)
            {
                await _httpContextAccessor.HttpContext.SignOutAsync();
            }

            if (loginProvider == null)
            {
                await SignInAsync(user, isPersistent, new Claim[] { new Claim("amr", "pwd") });
            }
            else
            {
                await SignInAsync(user, isPersistent, loginProvider);
            }

            return MyIdentityResult.SuccessResult;
        }

        public virtual Task SignInAsync(
            User user,
            bool isPersistent,
            string? authenticationMethod)
            => SignInAsync(user, new AuthenticationProperties { IsPersistent = isPersistent }, authenticationMethod);

        public virtual Task SignInAsync(
            User user, 
            AuthenticationProperties authenticationProperties,
            string? authenticationMethod = null)
        {
            IList<Claim> additionalClaims = new List<Claim>();
            if (authenticationMethod != null)
            {
                additionalClaims.Add(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod));
            }
            
            _db.Claims.Where(c => c.User == user).ToList()
                .ForEach(c => additionalClaims.Add(new Claim(c.Type, c.Value)));

            return SignInAsync(user, authenticationProperties, additionalClaims);
        }

        public virtual Task SignInAsync(
            User user, 
            bool isPersistent, 
            IEnumerable<Claim> additionalClaims)
            => SignInAsync(user, new AuthenticationProperties { IsPersistent = isPersistent },
                additionalClaims);

        public virtual async Task SignInAsync(
            User user, 
            AuthenticationProperties authenticationProperties,
            IEnumerable<Claim> additionalClaims)
        {
            var userPrincipal = await CreateUserPrincipalAsync(user, IdentityConstants.ApplicationScheme);
            var identity = userPrincipal.Identities.First();
            
            foreach (var claim in additionalClaims)
            {
                identity.AddClaim(claim);
            }

            await _httpContextAccessor.HttpContext.SignInAsync(
                userPrincipal,
                authenticationProperties);
        }

        public virtual async Task LogoutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync();
        }

        public virtual async Task<ClaimsPrincipal> CreateUserPrincipalAsync(User user, string authenticationType) =>
            await _claimsFactory.CreateAsync(user, authenticationType);

        public virtual AuthenticationProperties ConfigureExternalAuthenticationProperties(
            string provider, string redirectUrl, Guid? userId = null)
        {
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Items[LoginProviderKey] = provider;
            if (userId != null)
            {
                properties.Items[XsrfKey] = userId.ToString().ToLower();
            }

            return properties;
        }

        public virtual async Task<MyExternalLoginInfo?> GetUserExternalLoginInfoAsync(Guid? expectedXsrf = null)
        {
            var auth = await _httpContextAccessor.HttpContext.AuthenticateAsync();
            var items = auth?.Properties?.Items;
            if (auth?.Principal == null || items == null || !items.ContainsKey(LoginProviderKey))
            {
                return null;
            }

            if (expectedXsrf != null)
            {
                if (!items.ContainsKey(XsrfKey))
                {
                    return null;
                }

                var userIdString = items[XsrfKey];
                Guid.TryParse(userIdString, out var userId);
                if (userId != expectedXsrf)
                {
                    return null;
                }
            }

            var providerKey = auth.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var provider = items[LoginProviderKey];
            if (providerKey == null || provider == null)
            {
                return null;
            }

            var providerDisplayName = (await GetExternalAuthenticationSchemesAsync())
                                      .FirstOrDefault(p => p.Name == provider)?.DisplayName
                                      ?? provider;

            return new MyExternalLoginInfo(auth.Principal, provider, providerKey, providerDisplayName)
            {
                AuthenticationTokens = auth.Properties.GetTokens(),
                AuthenticationProperties = auth.Properties
            };
        }

        public virtual async Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            var schemes = await _schemes.GetAllSchemesAsync();
            return schemes.Where(s => !string.IsNullOrEmpty(s.DisplayName));
        }

        public virtual bool IsLoggedIn(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return principal.Identities.Any(i => i.AuthenticationType == IdentityConstants.ApplicationScheme);
        }

        public virtual async Task<MyIdentityResult> SetAuthenticationClaim(User user, string type)
        {
            var claimType = $"auth-{type}";
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            /*
            var claims = new List<Claim>();
            claims.AddRange(
                _httpContextAccessor.HttpContext.User.Claims.Where(claim =>
                    claim.Type.StartsWith("auth-") && claim.Type != claimType));
            claims.Add(new Claim(claimType, now.ToString()));
            */

            var userPrincipal = _httpContextAccessor.HttpContext.User;
            var identity = userPrincipal.Identities.First();
            identity.TryRemoveClaim(identity.Claims.FirstOrDefault(c => c.Type == claimType));
            identity.AddClaim(new Claim(claimType, now.ToString()));
            
            await _httpContextAccessor.HttpContext.SignInAsync(
                IdentityConstants.ApplicationScheme,
                userPrincipal,
                new AuthenticationProperties());
            

            return MyIdentityResult.SuccessResult;
        }
    }
}

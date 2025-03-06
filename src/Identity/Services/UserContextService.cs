using System.Security.Claims;
using Identity.Services;

namespace Identity.Services
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private bool _userIdLoaded;
        private Guid? _userId;
        
        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public Guid? UserId
        {
            get
            {
                if (_userIdLoaded)
                {
                    return _userId;
                }

                var userIdString =
                    _httpContextAccessor.HttpContext?.User.Claims
                        .FirstOrDefault(claim => claim.Type == "sub")?.Value
                    ?? _httpContextAccessor.HttpContext?.User.Claims
                        .FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

                if (Guid.TryParse(userIdString, out var userId))
                {
                    _userId = userId;
                    _userIdLoaded = true;
                }

                return _userId;
            }
        }

        public string? BearerToken
        {
            get
            {
                const string authorizationHeaderBearer = "Bearer";

                var authorizationHeader =
                    _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                if (authorizationHeader != null)
                {
                    var header = authorizationHeader.Trim();
                    if (header.StartsWith(authorizationHeaderBearer))
                    {
                        return header.Substring(authorizationHeaderBearer.Length).Trim();
                    }
                }

                return null;
            }
        }

        public string? PublicKey =>
            _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(item => item.Type == "public_key")?.Value;

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        }

        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;
        }
    }
}

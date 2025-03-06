using System.Text.Json.Serialization;
using Identity.Services;
using MediatR;
using Services;

namespace Microsoft.Extensions.DependencyInjection.Features;

public class GetUserInfoFeature
{
    public class Request : IRequest<Response>
    {
        
    }
    
    public class Response
    {
        [JsonPropertyName("sub")]
        public string? Subject { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }
    
     public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IUserContextService _userContextService;
        private readonly HttpContext? _httpContext;
        private readonly ITokenService _tokenService;

        const string AuthorizationHeaderBearer = "Bearer";

        public Handler(IUserContextService userContextService, IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
        {
            _userContextService = userContextService;
            _httpContext = httpContextAccessor?.HttpContext;
            _tokenService = tokenService;
        }

        public async Task<Response> Handle(Request request, CancellationToken ct)
        {
            Console.WriteLine("User Info Request");

            var response = new Response();

            if (_httpContext == null)
            {
                throw new InvalidOperationException();
            }

            var authenticated = _userContextService.IsAuthenticated();
            if (authenticated)
            {
                response.Subject = _userContextService.UserId?.ToString().ToLower();
            }

            if (!authenticated)
            {
                var bearerToken = _userContextService.BearerToken;

                if (!string.IsNullOrWhiteSpace(bearerToken))
                {
                    // TODO: https://local-5.dev.alanleouk.net"; // TODO: We are the authority (DynamicAuthority)
                    var securityToken = _tokenService.ValidateToken(bearerToken, "https://local-5.dev.alanleouk.net");
                    if (securityToken != null)
                    {
                        response.Subject = securityToken.Subject;
                        // TODO: User claims
                        authenticated = true;
                    }
                }
            }

            if (!authenticated)
            {
                response.Subject = _userContextService.UserId?.ToString().ToLower();
                // rw.Header().Set("WWW-Authenticate", "Bearer")
                _httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                // TODO: End response
            }

            return await Task.FromResult(response);
        }
    }
}
using System.Text.Json.Serialization;
using Identity.Constants;
using Identity.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Services;

namespace Microsoft.Extensions.DependencyInjection.Features;

public class GetEndSessionFeature
{
    public class Request : IRequest<Response>
    {
        [JsonPropertyName("id_token_hint")]
        public string? IdTokenHint { get; set; }

        [JsonPropertyName("post_logout_redirect_uri")]
        public string? PostLogoutRedirectUri { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }
    }

    public class Response
    {

    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IUserContextService _userContextService;
        private readonly ITokenService _tokenService;
        private readonly HttpContext? _httpContext;

        public Handler(IUserContextService userContextService, ITokenService tokenService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userContextService = userContextService;
            _tokenService = tokenService;
            _httpContext = httpContextAccessor?.HttpContext;
        }

        public async Task<Response> Handle(Request request, CancellationToken ct)
        {
            Console.WriteLine("End Session Request");

            var response = new Response();
            if (_httpContext == null)
            {
                return response;
            }

            // TODO: Check ID Token Hint
            // TODO: Add State
            
            await _httpContext.SignOutAsync();

            if (request.PostLogoutRedirectUri != null)
            {
                _httpContext.Response.Redirect(request.PostLogoutRedirectUri, true);
            }

            return await Task.FromResult(response);
        }
    }
}

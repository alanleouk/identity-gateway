using System.Text.Json.Serialization;
using System.Web;
using Identity.Extensions;
using Identity.Services;
using MediatR;

namespace Microsoft.Extensions.DependencyInjection.Features;

public class GetAuthorizeFeature
{
    public class Request : IRequest<Response>
    {
        [JsonPropertyName("response_type")]
        public string? ResponseType { get; set; }

        [JsonPropertyName("response_mode")]
        public string? ResponseMode { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }

        [JsonPropertyName("client_id")]
        public string? ClientId { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("redirect_uri")]
        public string? RedirectUri { get; set; }

        [JsonPropertyName("nonce")]
        public string? Nonce { get; set; }

        [JsonPropertyName("request_uri")]
        public string? RequestUri { get; set; }

        // [JsonPropertyName("request")]
        // public string? RequestProperty { get; set; }
    }

    public class Response
    {

    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IUserContextService _userContextService;
        private readonly ITokenService _tokenService;
        private readonly HttpContext? _httpContext;

        public Handler(IUserContextService userContextService,
            ITokenService tokenService, IHttpContextAccessor httpContextAccessor)
        {
            _userContextService = userContextService;
            _tokenService = tokenService;
            _httpContext = httpContextAccessor?.HttpContext;
        }

        public async Task<Response> Handle(Request request, CancellationToken ct)
        {
            Console.WriteLine("Authorize Request");

            var response = new Response();
            if (_httpContext == null)
            {
                return response;
            }

            var userId = _userContextService.UserId;
            // userId = new Guid("ba2bff88-d68b-47e1-ab22-2a5ecc5eddd1"); // TODO: Debug
            var email = "mail@todo.local";
            var scopes = new List<string>();
            if (request?.Scope != null)
            {
                scopes.AddRange(request.Scope.Split(' ', ','));
            }
            // scopes.Add("identity_api"); // TODO: Identity API Claim

            if (request == null || request?.ClientId == null || userId == null)
            {
                return response;
            }

            var authorized = _userContextService.IsAuthenticated();
            // authorized = true; // TODO: Debug

            var tokenServiceRequest = new TokenServiceRequest();
            tokenServiceRequest.ClientId = request.ClientId;
            tokenServiceRequest.Email = email;
            tokenServiceRequest.Subject = userId.Value.ToString().ToLower();
            tokenServiceRequest.Scopes = scopes;
            tokenServiceRequest.Nonce = request.Nonce;

            var accessTokenRequired = request.ResponseType == "token" || request.ResponseType == "id_token token";
            // TODO: https://local-5.dev.alanleouk.net"; // TODO: We are the authority (DynamicAuthority)
            var accessToken = accessTokenRequired
                ? _tokenService.CreateAccessToken(tokenServiceRequest, "https://local-5.dev.alanleouk.net")
                : null;

            var idTokenRequired = request.ResponseType == "id_token" || request.ResponseType == "id_token token";
            var idToken = idTokenRequired ? _tokenService.CreateIdToken(tokenServiceRequest, accessToken) : null;

            if (authorized)
            {
                switch (request.ResponseType)
                {
                    case "code":
                        var code = "TODO: Code";
                        var codeLocation =
                            $"{request.RedirectUri}?state={request.State}&scope={HttpUtility.UrlEncode(request.Scope)}&code={code}";
                        _httpContext.Response.Redirect(codeLocation, false);
                        break;

                    case "token":
                    case "id_token":
                    case "id_token token":
                        var items = new Dictionary<string, string>();
                        if (request.State != null)
                        {
                            items.Add("state", request.State);
                        }

                        if (request.Scope != null)
                        {
                            items.Add("scope", HttpUtility.UrlEncode(request.Scope));
                        }

                        items.Add("token_type", "Bearer");
                        // TODO: Configure token expiry
                        items.Add("expires_in", 3600.ToString());

                        if (idToken != null)
                        {
                            items.Add("id_token", idToken);
                        }

                        if (accessToken != null)
                        {
                            items.Add("access_token", accessToken);
                        }

                        switch (request.ResponseMode)
                        {
                            case "query":
                                var queryLocation = $"{request.RedirectUri}?{items.ToQueryParams(false)}";
                                // Console.WriteLine(queryLocation);
                                _httpContext.Response.Redirect(queryLocation, false);
                                break;

                            case "form_post":
                                var html =
                                    $"<html><head><title>Form Post</title></head><body onload=\"javascript: document.forms[0].submit()\"><form method=\"post\" action=\"{request.RedirectUri}\">{items.ToFormFields()}</form></body></html>";
                                // Console.WriteLine(html);
                                await _httpContext.Response.WriteAsync(html, ct);
                                await _httpContext.Response.CompleteAsync();
                                break;

                            case "fragment":
                            default:
                                var fragmentLocation = $"{request.RedirectUri}#{items.ToQueryParams(false)}";
                                // Console.WriteLine(fragmentLocation);
                                _httpContext.Response.Redirect(fragmentLocation, false);
                                break;
                        }

                        break;

                    default:
                        // TODO: Default
                        break;
                }
            }

            return await Task.FromResult(response);
        }
    }
}
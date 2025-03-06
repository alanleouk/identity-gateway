using System.Text.Json.Serialization;
using Identity.Configuration;
using MediatR;

namespace Microsoft.Extensions.DependencyInjection.Features;

public class GetConfigurationFeature
{
    public class Request : IRequest<Response>
    {

    }

    public class Response
    {
        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        // Endpoints

        [JsonPropertyName("jwks_uri")]
        public string JwksUri { get; set; }

        [JsonPropertyName("authorization_endpoint")]
        public string AuthorizationEndpoint { get; set; }

        [JsonPropertyName("token_endpoint")]
        public string TokenEndpoint { get; set; }

        [JsonPropertyName("userinfo_endpoint")]
        public string UserinfoEndpoint { get; set; }

        [JsonPropertyName("end_session_endpoint")]
        public string EndSessionEndpoint { get; set; }

        [JsonPropertyName("check_session_iframe")]
        public string CheckSessionIframe { get; set; }

        [JsonPropertyName("revocation_endpoint")]
        public string RevocationEndpoint { get; set; }

        [JsonPropertyName("introspection_endpoint")]
        public string IntrospectionEndpoint { get; set; }

        [JsonPropertyName("device_authorization_endpoint")]
        public string DeviceAuthorizationEndpoint { get; set; }

        // Channels

        [JsonPropertyName("frontchannel_logout_supported")]
        public bool FrontchannelLogoutSupported { get; set; }

        [JsonPropertyName("frontchannel_logout_session_supported")]
        public bool FrontchannelLogoutSessionSupported { get; set; }

        [JsonPropertyName("backchannel_logout_supported")]
        public bool BackchannelLogoutSupported { get; set; }

        [JsonPropertyName("backchannel_logout_session_supported")]
        public bool BackchannelLogoutSessionSupported { get; set; }

        // Other

        [JsonPropertyName("scopes_supported")]
        public IList<string> ScopesSupported { get; set; }

        [JsonPropertyName("claims_supported")]
        public IList<string> ClaimsSupported { get; set; }

        [JsonPropertyName("grant_types_supported")]
        public IList<string> GrantTypesSupported { get; set; }

        [JsonPropertyName("response_types_supported")]
        public IList<string> ResponseTypesSupported { get; set; }

        [JsonPropertyName("response_modes_supported")]
        public IList<string> ResponseModesSupported { get; set; }

        [JsonPropertyName("token_endpoint_auth_methods_supported")]
        public IList<string> TokenEndpointAuthMethodsSupported { get; set; }

        [JsonPropertyName("id_token_signing_alg_values_supported")]
        public IList<string> IdTokenSigningAlgValuesSupported { get; set; }

        [JsonPropertyName("subject_types_supported")]
        public IList<string> SubjectTypesSupported { get; set; }

        [JsonPropertyName("code_challenge_methods_supported")]
        public IList<string> CodeChallengeMethodsSupported { get; set; }

        [JsonPropertyName("request_parameter_supported")]
        public bool RequestParameterSupported { get; set; }

        public Response(string issuer)
        {
            Issuer = issuer;
            JwksUri = $"{issuer}{EndpointConstants.Jwks}";
            AuthorizationEndpoint = $"{issuer}{EndpointConstants.Authorization}";
            TokenEndpoint = $"{issuer}{EndpointConstants.Token}";
            UserinfoEndpoint = $"{issuer}{EndpointConstants.Userinfo}";
            EndSessionEndpoint = $"{issuer}{EndpointConstants.EndSession}";
            CheckSessionIframe = $"{issuer}{EndpointConstants.CheckSession}";
            RevocationEndpoint = $"{issuer}{EndpointConstants.Revocation}";
            IntrospectionEndpoint = $"{issuer}{EndpointConstants.Introspection}";
            DeviceAuthorizationEndpoint = $"{issuer}{EndpointConstants.DeviceAuthorization}";

            FrontchannelLogoutSupported = true;
            FrontchannelLogoutSessionSupported = true;
            BackchannelLogoutSupported = true;
            BackchannelLogoutSessionSupported = true;

            ScopesSupported = new List<string>
            {
                "openid",
                "profile",
                "email",
                "roles",
            };
            ClaimsSupported = new List<string>
            {
                "sub",
                "name",
                "profile",
                "email",
                "role",
            };
            GrantTypesSupported = new List<string>
            {
                "authorization_code",
                "client_credentials",
                "refresh_token",
                "implicit",
                "password",
                // "urn:ietf:params:oauth:grant-type:device_code",
            };
            ResponseTypesSupported = new List<string>
            {
                "code",
                "token",
                "id_token",
                "id_token token",
                "code id_token",
                "code token",
                "code id_token token",
            };
            ResponseModesSupported = new List<string>
            {
                "form_post",
                "query",
                "fragment",
            };
            TokenEndpointAuthMethodsSupported = new List<string>
            {
                // "client_secret_basic",
                // "client_secret_post",
            };
            IdTokenSigningAlgValuesSupported = new List<string>
            {
                "RS256",
            };
            SubjectTypesSupported = new List<string>
            {
                "public",
            };
            CodeChallengeMethodsSupported = new List<string>
            {
                "plain",
                "S256",
            };

            RequestParameterSupported = true;
        }
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken ct)
        {
            Console.WriteLine("Configuration Request");

            // TODO: https://local-5.dev.alanleouk.net"; // TODO: We are the authority (DynamicAuthority)
            
            var response = new Response("https://local-5.dev.alanleouk.net");
            return await Task.FromResult(response);
        }
    }
}
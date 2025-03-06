using System.Text.Json.Serialization;
using MediatR;

namespace Microsoft.Extensions.DependencyInjection.Features;

// See: https://datatracker.ietf.org/doc/html/rfc7662

public class PostIntrospectionFeature
{
    public class Request : IRequest<Response>
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("token_type_hint")]
        public string? TokenTypeHint { get; set; }
    }
    
    public class Response
    {
        [JsonPropertyName("active")]
        public bool? Active { get; set; }
    }
    
    public async Task<Response> Handle(Request request, CancellationToken ct)
    {
        Console.WriteLine("Introspection Request");

        var response = new Response
        {
            Active = true
        };

        return await Task.FromResult(response);
    }
}

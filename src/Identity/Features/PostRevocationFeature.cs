using System.Text.Json.Serialization;
using MediatR;

namespace Microsoft.Extensions.DependencyInjection.Features;

// See: https://datatracker.ietf.org/doc/html/rfc7009

public class PostRevocationFeature
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
       
    }
    
    public class Handler : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken ct)
        {
            Console.WriteLine("Revocation Request");

            var response = new Response();

            return await Task.FromResult(response);
        }
    }
}

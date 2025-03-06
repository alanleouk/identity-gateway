using Identity.Models;
using MediatR;
using Services;

namespace Microsoft.Extensions.DependencyInjection.Features;

public class GetJwksFeature
{
    public class Request : IRequest<Response>
    {
        
    }
    
    public class Response
    {
        public required IList<PublicJsonWebKey> Keys { get; set; }
    }
    
    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IPublicCredentialsService _publicCredentialService;

        public Handler(IPublicCredentialsService publicCredentialService)
        {
            _publicCredentialService = publicCredentialService;
        }

        public async Task<Response> Handle(Request request, CancellationToken ct)
        {
            Console.WriteLine("Jwks Request");

            var response = new Response
            {
                Keys = _publicCredentialService.PublicJsonWebKeys()
            };
            return await Task.FromResult(response);
        }
    }
}

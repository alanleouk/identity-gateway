using Jwks.Models;
using Jwks.Services;
using MediatR;

namespace Jwks;

public class GetJwksFeature
{
    public class Request : IRequest<Response>
    {
        public string? Kid { get; set; }
    }

    public class Response
    {
        public required IList<PublicJsonWebKey>? Keys { get; set; }
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

            // TODO: Optimise
            if (!string.IsNullOrEmpty(request.Kid))
            {
                response.Keys = response.Keys.Where(key => key.Kid == request.Kid).ToList();
            }

            return await Task.FromResult(response);
        }
    }
}

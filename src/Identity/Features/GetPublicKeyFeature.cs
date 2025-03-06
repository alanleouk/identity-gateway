using System.Security.Cryptography;
using Identity.Models;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using Services;

namespace Microsoft.Extensions.DependencyInjection.Features;

public class GetPublicKeyFeature
{
    public class Request : IRequest<Response>
    {

    }

    public class Response
    {
        public PublicJsonWebKey? PublicJsonWebKey { get; set; }
        public byte[]? PublicKeyBlob { get; set; }
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
            Console.WriteLine("Public Key Request");

            var response = new Response();

            var keys = _publicCredentialService.PublicJsonWebKeys();
            var jwk = keys.FirstOrDefault(item => item.Crv != null);
            if (jwk != null)
            {
                response.PublicJsonWebKey = jwk;

                var curve = jwk.Crv switch
                {
                    "P-256" => ECCurve.NamedCurves.nistP256,
                    "P-384" => ECCurve.NamedCurves.nistP384,
                    "P-521" => ECCurve.NamedCurves.nistP521,
                    _ => throw new NotSupportedException()
                };

                var key = ECDiffieHellman.Create(new ECParameters
                {
                    Curve = curve,
                    Q = new ECPoint
                    {
                        X = Base64UrlEncoder.DecodeBytes(jwk.X),
                        Y = Base64UrlEncoder.DecodeBytes(jwk.Y)
                    }
                });
                response.PublicKeyBlob = key.PublicKey.ToByteArray();
            }

            return response;
        }
    }
}

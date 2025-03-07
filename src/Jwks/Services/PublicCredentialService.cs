using Jwks.Models;

namespace Jwks.Services
{
    public interface IPublicCredentialsService
    {
        public IList<PublicJsonWebKey> PublicJsonWebKeys();
    }

    public class PublicCredentialService : IPublicCredentialsService
    {
        public readonly ICredentialsService _credentialsService;

        public PublicCredentialService(ICredentialsService credentialsService)
        {
            _credentialsService = credentialsService;
        }

        public IList<PublicJsonWebKey> PublicJsonWebKeys()
        {
            return _credentialsService.JsonWebKeys().Select(item => new PublicJsonWebKey
                {
                    Alg = item.Alg,
                    Crv = item.Crv,
                    E = item.E,
                    Kid = item.Kid,
                    Kty = item.Kty,
                    N = item.N,
                    Use = item.Use,
                    X = item.X,
                    Y = item.Y
                })
                .ToList();
        }
    }
}
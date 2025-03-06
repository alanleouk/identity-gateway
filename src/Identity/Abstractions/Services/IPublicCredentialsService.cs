
using Identity.Models;

namespace Services
{
    public interface IPublicCredentialsService
    {
        public IList<PublicJsonWebKey> PublicJsonWebKeys();
    }
}

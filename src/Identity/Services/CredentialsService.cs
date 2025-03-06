using Microsoft.IdentityModel.Tokens;

namespace Identity.Services
{
    public interface ICredentialsService
    {
        public void Add(JsonWebKey jsonWebKey);
        public JsonWebKey? Find(string keyId);
        IList<JsonWebKey> JsonWebKeys();
    }

    public class CredentialsService : ICredentialsService
    {
        private readonly Dictionary<string, JsonWebKey> _credentials = new();

        public void Add(JsonWebKey jsonWebKey)
        {
            if (!_credentials.ContainsKey(jsonWebKey.KeyId))
            {
                _credentials.Add(jsonWebKey.KeyId, jsonWebKey);
            }
        }

        public JsonWebKey? Find(string keyId)
        {
            _credentials.TryGetValue(keyId, out JsonWebKey? jsonWebKey);
            return jsonWebKey;
        }

        public IList<JsonWebKey> JsonWebKeys()
        {
            return _credentials.Values.ToList();
        }
    }
}

namespace Identity.Models.FindModels
{
    public class ByExternalLogin
    {
        public string Provider { get; }
        public string Key { get; }

        public ByExternalLogin(string provider, string key)
        {
            Provider = provider;
            Key = key;
        }
    }
}

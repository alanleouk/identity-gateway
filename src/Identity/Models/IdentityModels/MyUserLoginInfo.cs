namespace Identity.Models.IdentityModels
{
    public class MyUserLoginInfo
    {
        public MyUserLoginInfo(string loginProvider, string providerKey, string displayName)
        {
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
            DisplayName = displayName;
        }

        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string DisplayName { get; set; }
        public string? PublicKey { get; set; }
    }
}

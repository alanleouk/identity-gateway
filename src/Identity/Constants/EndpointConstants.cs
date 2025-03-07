namespace Identity.Configuration
{
    public static class EndpointConstants
    {
        public const string Configuration = "/.well-known/openid-configuration";
        public const string Authorization = "/connect/authorize";
        public const string Token = "/connect/token";
        public const string Userinfo = "/connect/userinfo";
        public const string EndSession = "/connect/endsession";
        public const string CheckSession = "/connect/checksession";
        public const string Revocation = "/connect/revocation";
        public const string Introspection = "/connect/introspect";
        public const string DeviceAuthorization = "/connect/deviceauthorization";
    }
}

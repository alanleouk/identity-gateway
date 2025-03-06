using Microsoft.AspNetCore.Authentication;

namespace Identity.Models.AccountModels
{
    public class InfoModel
    {
        public string? Email { get; set; }
        public string? Roles { get; set; }
        public string? Certificates { get; set; }
        public IList<LoginProviderModel>? Logins { get; set; }
        public IList<AuthenticationScheme>? OtherLogins { get; set; }
    }
}

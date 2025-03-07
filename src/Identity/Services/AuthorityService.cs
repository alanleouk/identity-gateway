using Microsoft.IdentityModel.Protocols.Configuration;

namespace Identity.Services;

public class AuthorityService
{
    // TODO: Update to use HashSet
    public string PrimaryAuthority { get; }
    public List<string> AllowedAuthorities { get; }

    public AuthorityService(IConfigurationSection configurationSection)
    {
        PrimaryAuthority = configurationSection.GetSection("PrimaryAuthority").Get<string>()
                           ?? throw new InvalidConfigurationException("PrimaryAuthority is required");
        AllowedAuthorities = configurationSection.GetSection("AllowedAuthorities").Get<List<string>>()
                             ?? throw new InvalidConfigurationException("AllowedAuthorities is required");

        if (!AllowedAuthorities.Contains(PrimaryAuthority))
        {
            throw new InvalidConfigurationException("PrimaryAuthority must be in AllowedAuthorities");
        }
    }
}

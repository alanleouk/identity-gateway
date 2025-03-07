using Microsoft.Extensions.Configuration;

namespace Authority.Services;

public class AuthorityService
{
    // TODO: Update to use HashSet
    public string PrimaryAuthority { get; }
    public List<string> AllowedAuthorities { get; }

    public AuthorityService(IConfigurationSection configurationSection)
    {
        PrimaryAuthority = configurationSection.GetSection("PrimaryAuthority").Get<string>()
                           ?? throw new InvalidOperationException("PrimaryAuthority is required");
        AllowedAuthorities = configurationSection.GetSection("AllowedAuthorities").Get<List<string>>()
                             ?? throw new InvalidOperationException("AllowedAuthorities is required");

        if (!AllowedAuthorities.Contains(PrimaryAuthority))
        {
            throw new InvalidOperationException("PrimaryAuthority must be in AllowedAuthorities");
        }
    }
}

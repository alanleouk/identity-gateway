using Microsoft.Extensions.Configuration;

namespace Authority.Services;

public class AuthorityConfig
{
    // TODO: Update to use HashSet
    public string PrimaryAuthority { get; }
    public List<string> Authorities { get; }
    public List<string> TrustedAuthorities { get; }

    public AuthorityConfig(IConfigurationSection configurationSection)
    {
        Authorities = configurationSection
                          .GetSection("Authorities")
                          .Get<List<string>>()
                          ?.Select(item => item.TrimEnd('/'))
                          .ToList()
                      ?? [];

        if (Authorities.Count == 0)
        {
            throw new InvalidOperationException("At least one authority must be defined");
        }

        PrimaryAuthority = Authorities.First();
        
        TrustedAuthorities = configurationSection
                                 .GetSection("TrustedAuthorities")
                                 .Get<List<string>>()
                                 ?.Select(item => item.TrimEnd('/'))
                                 .ToList()
                             ?? [];
    }
}

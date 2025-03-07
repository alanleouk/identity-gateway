using Microsoft.IdentityModel.Protocols.Configuration;

namespace Identity.Services;

public class TrustedKeyService
{
    private List<string> TrustedKeys { get; }

    public TrustedKeyService(IConfigurationSection configurationSection)
    {
        TrustedKeys = configurationSection.Get<List<string>>()
                      ?? throw new InvalidConfigurationException("TrustedKeys section is required");
    }
}

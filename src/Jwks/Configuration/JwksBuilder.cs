
using Microsoft.Extensions.DependencyInjection;

namespace Jwks.Configuration
{
    public class JwksBuilder : IJwksBuilder
    {
        public JwksBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }
    }
}

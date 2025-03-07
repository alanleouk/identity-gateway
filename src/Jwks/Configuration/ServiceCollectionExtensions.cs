using MediatR;
using Jwks.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        private static IJwksBuilder AddJwksBuilder(this IServiceCollection services)
        {
            return new JwksBuilder(services);
        }

        public static IJwksBuilder AddJwksServices(this IServiceCollection services)
        {
            var builder = services.AddJwksBuilder();
            builder.Services.MyAddMediatR("Jwks");
            return builder;
        }
    }
}
using Microsoft.Extensions.DependencyInjection;

namespace MediatR;

public static class MyExtensions
{
    public static IServiceCollection MyAddMediatR(this IServiceCollection services, params string[] assemblyNames)
    {
        var assemblyList = assemblyNames.Select(assemblyName => AppDomain.CurrentDomain.Load(assemblyName)).ToArray();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblyList));
        return services;
    }
}

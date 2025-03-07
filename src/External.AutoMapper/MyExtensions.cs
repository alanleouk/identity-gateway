using Microsoft.Extensions.DependencyInjection;

namespace AutoMapper;

public static class MyExtensions
{
    public static IServiceCollection MyAddAutoMapper(this IServiceCollection services, params string[] assemblyNames)
    {
        var assemblyList = assemblyNames.Select(assemblyName => AppDomain.CurrentDomain.Load(assemblyName)).ToArray();
        services.AddAutoMapper(assemblyList);
        return services;
    }
}

using Giphy.Application.Interfaces.Repositories;
using Giphy.Persistance.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Giphy.Persistance.Extensions;

public static class DependencyExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(config.GetConnectionString("Redis")));
        services.AddScoped<IGifRepository, GifRepository>();
        return services;
    }

    public static IHealthChecksBuilder AddPersistanceHealthChecks(this IHealthChecksBuilder healtcheck, IConfiguration configuration)
    {
        return healtcheck.AddRedis(configuration.GetConnectionString("Redis"));
    }
}
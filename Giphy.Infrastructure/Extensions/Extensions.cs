using Giphy.Application.Interfaces.Services;
using Giphy.Infrastructure.Configs;
using Giphy.Infrastructure.Mapping;
using Giphy.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Giphy.Infrastructure.Extensions;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddSingleton<IGifService, GifService>();

        services.AddHttpClient<GifService>();
        services.Configure<GiphyOptions>(config.GetSection(nameof(GiphyOptions)));

        return services;
    }
}
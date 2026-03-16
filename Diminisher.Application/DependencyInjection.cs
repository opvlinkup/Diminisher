using Application.Abstractions;
using Application.Abstractions.Generators;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUrlService, UrlService>();
        services.AddSingleton<IShortCodeGenerator, ShortCodeGenerator>();
        services.AddSingleton<IUrlHasher, UrlHasher>();
        return services;
    }
}
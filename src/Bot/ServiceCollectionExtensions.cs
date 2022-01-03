using Ivao.It.DiscordBot.ClientEventsHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ivao.It.DiscordBot;

public static class ServiceCollectionExtensions
{
    private static void InjectDI(this IServiceCollection services)
    {
        services.AddScoped<MessageCreatedEventHandlers>();
        services.AddSingleton<IvaoItBot>();
    }

    public static IServiceCollection AddIvaoItBot(this IServiceCollection services, Action<DiscordConfig> configAction)
    {
        services.InjectDI();
        services.Configure<DiscordConfig>(configAction);
        return services;
    }

    public static IServiceCollection AddIvaoItBot(this IServiceCollection services, IConfiguration configSection)
    {
        services.InjectDI();
        services.Configure<DiscordConfig>(configSection);
        return services;
    }
}

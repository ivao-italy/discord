using Ivao.It.Discord.Shared.Services;
using Ivao.It.DiscordBot.DiscordEventsHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ivao.It.DiscordBot;

public static class ServiceCollectionExtensions
{
    private static void InjectDI(this IServiceCollection services)
    {
        services.AddScoped<MessageCreatedEventHandlers>();
        services.AddScoped<CommandsNextEventHandlers>();
        services.AddScoped<ITrainingAndExamsService, TrainingAndExamsService>();

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

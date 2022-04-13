using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.DependencyInjection;

namespace Ivao.It.DiscordBot.Commands;

internal static class DiscordClientExtensions_Commands
{
    internal static DiscordClient UseIvaoCommands(this DiscordClient client, IServiceScopeFactory serviceScopeFactory)
    {
        var commands = client.UseCommandsNext(new CommandsNextConfiguration
        {
            EnableMentionPrefix = true,
            Services = serviceScopeFactory.CreateScope().ServiceProvider
        });

        //Commands
        commands.RegisterCommands<BotCommands>();
        commands.RegisterCommands<EventsWorkflowCommands>();

        //Global Event Handlers
        commands.CommandExecuted += CommandsNextEventHandlers.Commands_CommandExecuted;
        commands.CommandErrored += CommandsNextEventHandlers.Commands_CommandErrored;

        return client;
    }
}
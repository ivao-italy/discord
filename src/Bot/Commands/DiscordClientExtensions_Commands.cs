using DSharpPlus;
using DSharpPlus.CommandsNext;
using Ivao.It.DiscordBot.Commands.EventsWorkflow;
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
        //commands.RegisterCommands(Assembly.GetExecutingAssembly()); //Events are not public.
        commands.RegisterCommands<BotCommands>();
        commands.RegisterCommands<EventsCommands>();
        
        //Global Event Handlers
        commands.CommandExecuted += CommandsNextEventHandlers.Commands_CommandExecuted;
        commands.CommandErrored += CommandsNextEventHandlers.Commands_CommandErrored;

        return client;
    }
}